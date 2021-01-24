using System;
using System.ComponentModel;

namespace DataGridFilterLibrary.Support
{
    [Serializable]
    public class FilterData : INotifyPropertyChanged
    {
        #region Metadata

        public FilterType Type { get; set; }
        public String ValuePropertyBindingPath { get; set; }
        public Type ValuePropertyType { get; set; }
        public bool IsTypeInitialized { get; set; }
        public bool IsCaseSensitiveSearch { get; set; }

        //query optimization fileds
        public bool IsSearchPerformed { get; set; }
        public bool IsRefresh { get; set; }
        //query optimization fileds
        #endregion

        #region Filter Change Notification
        public event EventHandler<EventArgs> FilterChangedEvent;
        private bool _isClearData;

        private void OnFilterChangedEvent()
        {
            EventHandler<EventArgs> temp = FilterChangedEvent;

            if (temp != null)
            {
                bool filterChanged = false;

                switch (Type)
                {
                    case FilterType.Numeric:
                    case FilterType.DateTime:

                        filterChanged = (Operator != FilterOperator.Undefined || QueryString != String.Empty);
                        break;

                    case FilterType.NumericBetween:
                    case FilterType.DateTimeBetween:

                        _operator = FilterOperator.Between;
                        filterChanged = true;
                        break;

                    case FilterType.Text:
                    case FilterType.TextContains:

                        _operator = FilterOperator.Like;
                        filterChanged = true;
                        break;

                    case FilterType.List:
                    case FilterType.Boolean:

                        _operator = FilterOperator.Equals;
                        filterChanged = true;
                        break;

                    default:
                        filterChanged = false;
                        break;
                }

                if (filterChanged && !_isClearData) temp(this, EventArgs.Empty);
            }
        }
        #endregion
        public void ClearData()
        {
            _isClearData = true;

            Operator           = FilterOperator.Undefined;
            if (QueryString   != String.Empty) QueryString = null;
            if (QueryStringTo != String.Empty) QueryStringTo = null;

            _isClearData = false;
        }

        private FilterOperator _operator;
        public FilterOperator Operator
        {
            get { return _operator; }
            set
            {
                if(_operator != value)
                {
                    _operator = value;
                    NotifyPropertyChanged("Operator");
                    OnFilterChangedEvent();
                }
            }
        }

        private string _queryString;
        public string QueryString
        {
            get 
            {
                 return _queryString;
            }
            set
            {
                if (_queryString != value)
                {
                    _queryString = value;

                    if (_queryString == null) _queryString = String.Empty;

                    NotifyPropertyChanged("QueryString");
                    OnFilterChangedEvent();
                }
            }
        }

        private string _queryStringTo;
        public string QueryStringTo
        {
            get { return _queryStringTo; }
            set
            {
                if (_queryStringTo != value)
                {
                    _queryStringTo = value;

                    if (_queryStringTo == null) _queryStringTo = String.Empty;

                    NotifyPropertyChanged("QueryStringTo");
                    OnFilterChangedEvent();
                }
            }
        }

        public FilterData(
            FilterOperator Operator,
            FilterType Type,
            String ValuePropertyBindingPath,
            Type ValuePropertyType,
            String QueryString,
            String QueryStringTo,
            bool IsTypeInitialized,
            bool IsCaseSensitiveSearch
            )
        {
            this.Operator = Operator;
            this.Type = Type;
            this.ValuePropertyBindingPath = ValuePropertyBindingPath;
            this.ValuePropertyType = ValuePropertyType;
            this.QueryString   = QueryString;
            this.QueryStringTo = QueryStringTo;

            this.IsTypeInitialized    = IsTypeInitialized;
            this.IsCaseSensitiveSearch = IsCaseSensitiveSearch;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
