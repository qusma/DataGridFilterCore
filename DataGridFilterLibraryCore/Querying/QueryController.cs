using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using DataGridFilterLibrary.Support;
using System.Collections;
using System.Windows.Data;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace DataGridFilterLibrary.Querying
{
    public class QueryController
    {
        public FilterData  ColumnFilterData { get; set; }
        public IEnumerable ItemsSource { get; set; }

        private readonly Dictionary<string, FilterData> _filtersForColumns;

        private Query _query;

        public Dispatcher       CallingThreadDispatcher { get; set; }
        public bool             UseBackgroundWorker { get; set; }
        private readonly object _lockObject;

        public QueryController()
        {
            _lockObject = new object();

            _filtersForColumns = new Dictionary<string, FilterData>();
            _query = new Query();
        }

        public void DoQuery()
        {
            DoQuery(false);
        }

        public void DoQuery(bool force)
        {
            ColumnFilterData.IsSearchPerformed = false;

            if (!_filtersForColumns.ContainsKey(ColumnFilterData.ValuePropertyBindingPath))
            {
                _filtersForColumns.Add(ColumnFilterData.ValuePropertyBindingPath, ColumnFilterData);
            }
            else
            {
                _filtersForColumns[ColumnFilterData.ValuePropertyBindingPath] = ColumnFilterData;
            }

            if (isRefresh)
            {
                if (_filtersForColumns.ElementAt(_filtersForColumns.Count - 1).Value.ValuePropertyBindingPath
                    == ColumnFilterData.ValuePropertyBindingPath)
                {
                    runFiltering(force);
                }
            }
            else if (filteringNeeded)
            {
                runFiltering(force);
            }

            ColumnFilterData.IsSearchPerformed = true;
            ColumnFilterData.IsRefresh = false;
        }

        public bool IsCurentControlFirstControl
        {
            get
            {
                return _filtersForColumns.Count > 0
                        ? _filtersForColumns.ElementAt(0).Value.ValuePropertyBindingPath == ColumnFilterData.ValuePropertyBindingPath 
                        : false;
            }
        }

        public void ClearFilter()
        {
            int count = _filtersForColumns.Count;
            for(int i = 0; i < count; i++)
            {
                FilterData data = _filtersForColumns.ElementAt(i).Value;

                data.ClearData();
            }

            DoQuery();
        }

        #region Internal

        private bool isRefresh
        {
            get { return (from f in _filtersForColumns where f.Value.IsRefresh == true select f).Count() > 0; }
        }

        private bool filteringNeeded
        {
            get { return (from f in _filtersForColumns where f.Value.IsSearchPerformed == false select f).Count() == 1; }
        }

        private void runFiltering(bool force)
        {
            bool filterChanged;

            CreateFilterExpressionsAndFilteredCollection(out filterChanged, force);

            if (filterChanged || force)
            {
                OnFilteringStarted(this, EventArgs.Empty);

                applayFilter();
            }
        }

        private void CreateFilterExpressionsAndFilteredCollection(out bool filterChanged, bool force)
        {
            QueryCreator queryCreator = new QueryCreator(_filtersForColumns);

            queryCreator.CreateFilter(ref _query);

            filterChanged = (_query.IsQueryChanged || (_query.FilterString != String.Empty && isRefresh));

            if ((force && _query.FilterString != String.Empty) || (_query.FilterString != String.Empty && filterChanged))
            {
                IEnumerable collection = ItemsSource;

                if (ItemsSource is ListCollectionView)
                {
                    collection = (ItemsSource as ListCollectionView).SourceCollection;
                }

                var observable = ItemsSource as System.Collections.Specialized.INotifyCollectionChanged;
                if (observable != null)
                {
                    observable.CollectionChanged -= observable_CollectionChanged;
                    observable.CollectionChanged += observable_CollectionChanged;

                }

                #region Debug
                #if DEBUG
                Debug.WriteLine("QUERY STATEMENT: " + _query.FilterString);

                string debugParameters = String.Empty;
                _query.QueryParameters.ForEach(p =>
                {
                    if (debugParameters.Length > 0) debugParameters += ",";
                    if (p!=null) debugParameters += p.ToString();
                });

                Debug.WriteLine("QUERY PARAMETRS: " + debugParameters);
                #endif
                #endregion

                if (_query.FilterString != String.Empty)
                {
                    var result = collection.AsQueryable().Where(_query.FilterString, _query.QueryParameters.ToArray<object>());

                    filteredCollection = result.Cast<object>().ToList();
                }
            }
            else
            {
                filteredCollection = null;
            }

            _query.StoreLastUsedValues();
        }

        //private void observable_CollectionChanged(
        //    object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    DoQuery(true);
        //}

        private void observable_CollectionChanged(
     object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            bool collectionIsCollectionViewAndIsFiltered = false;

            ListCollectionView view = sender as ListCollectionView;
            if (view != null && view.SourceCollection is IList && view.Groups != null)
            {
                int filteredItemsCount = view.Count;
                int sourceItemsCount = (view.SourceCollection as IList).Count;

                collectionIsCollectionViewAndIsFiltered = filteredItemsCount != sourceItemsCount
                    && view.Groups.Count > 0;
            }

            if (!collectionIsCollectionViewAndIsFiltered && e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Reset
                )//already handled and prevent unnecessary filtering
            {
                DoQuery(true);
            }
        }

        #region Internal Filtering

        private IList filteredCollection;
        HashSet<object> filteredCollectionHashSet;

        void applayFilter()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);

            if (filteredCollection != null)
            {
                executeFilterAction(
                    new Action(() =>
                    {
                        filteredCollectionHashSet = initLookupDictionary(filteredCollection);
 
                        view.Filter = new Predicate<object>(itemPassesFilter);

                        OnFilteringFinished(this, EventArgs.Empty);
                    })
                );
            }
            else
            {
                executeFilterAction(
                    new Action(() =>
                    {
                        if (view.Filter != null)
                        {
                            view.Filter = null;
                        }

                        OnFilteringFinished(this, EventArgs.Empty);
                    })
                );
            }
        }

        private void executeFilterAction(Action action)
        {
            if (UseBackgroundWorker)
            {
                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += delegate(object sender, DoWorkEventArgs e)
                {
                    lock (_lockObject)
                    {
                        executeActionUsingDispatcher(action);
                    }
                };

                worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                {
                    if (e.Error != null)
                    {
                        OnFilteringError(this, new FilteringEventArgs(e.Error));
                    }
                };

                worker.RunWorkerAsync();
            }
            else
            {
                try
                {
                    executeActionUsingDispatcher(action);
                }
                catch (Exception e)
                {
                    OnFilteringError(this, new FilteringEventArgs(e));
                }
            }
        }

        private void executeActionUsingDispatcher(Action action)
        {
            if (this.CallingThreadDispatcher != null && !this.CallingThreadDispatcher.CheckAccess())
            {
                this.CallingThreadDispatcher.Invoke
                    (
                        new Action(() =>
                        {
                            invoke(action);
                        })
                    );
            }
            else
            {
                invoke(action);
            }
        }

        private static void invoke(Action action)
        {
            System.Diagnostics.Trace.WriteLine("------------------ START APPLAY FILTER ------------------------------");
            Stopwatch sw = Stopwatch.StartNew();

            action.Invoke();

            sw.Stop();
            System.Diagnostics.Trace.WriteLine("TIME: " + sw.ElapsedMilliseconds);
            System.Diagnostics.Trace.WriteLine("------------------ STOP APPLAY FILTER ------------------------------");
        }

        private bool itemPassesFilter(object item)
        {
            return filteredCollectionHashSet.Contains(item);
        }

        #region Helpers
        private HashSet<object> initLookupDictionary(IList collection)
        {
            HashSet<object> dictionary;

            if (collection != null)
            {
                dictionary = new HashSet<object>(collection.Cast<object>()/*.ToList()*/);
            }
            else
            {
                dictionary = new HashSet<object>();
            }

            return dictionary;
        }
        #endregion

        #endregion
        #endregion

        #region Progress Notification
        public event EventHandler<EventArgs> FilteringStarted;
        public event EventHandler<EventArgs> FilteringFinished;
        public event EventHandler<FilteringEventArgs> FilteringError;

        private void OnFilteringStarted(object sender, EventArgs e)
        {
            EventHandler<EventArgs> localEvent = FilteringStarted;

            if (localEvent != null) localEvent(sender, e);
        }

        private void OnFilteringFinished(object sender, EventArgs e)
        {
            EventHandler<EventArgs> localEvent = FilteringFinished;

            if (localEvent != null) localEvent(sender, e);
        }

        private void OnFilteringError(object sender, FilteringEventArgs e)
        {
            EventHandler<FilteringEventArgs> localEvent = FilteringError;

            if (localEvent != null) localEvent(sender, e);
        }
        #endregion
    }
}
