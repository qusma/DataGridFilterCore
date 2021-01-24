using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections;
using DataGridFilterLibrary.Support;
using System.Reflection;
using DataGridFilterLibrary.Querying;
using System.Windows.Controls.Primitives;

namespace DataGridFilterLibrary
{
    public class DataGridColumnFilter : Control
    {
        static DataGridColumnFilter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridColumnFilter), new FrameworkPropertyMetadata(typeof(DataGridColumnFilter)));
        }

        public DataGridColumnFilter()
            : base()
        {
            this.GotFocus += new RoutedEventHandler(DataGridColumnFilter_GotFocus);
        }

        void DataGridColumnFilter_GotFocus(object sender, RoutedEventArgs e)
        {
            DataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            DataGrid.CommitEdit(DataGridEditingUnit.Row, true);
        }

        #region Overrides
        protected override void OnPropertyChanged(
            DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DataGridItemsSourceProperty
                && e.OldValue != e.NewValue
                && AssignedDataGridColumn != null && DataGrid != null)
            {
                Initialize();

                FilterCurrentData.IsRefresh = true;//query optimization filed

                filterCurrentData_FilterChangedEvent(this, EventArgs.Empty);//init query

                FilterCurrentData.FilterChangedEvent -= filterCurrentData_FilterChangedEvent;
                FilterCurrentData.FilterChangedEvent += filterCurrentData_FilterChangedEvent;
            }

            base.OnPropertyChanged(e);
        }
        #endregion

        #region Properties
        public FilterData FilterCurrentData
        {
            get { return (FilterData)GetValue(FilterCurrentDataProperty); }
            set { SetValue(FilterCurrentDataProperty, value); }
        }

        public static readonly DependencyProperty FilterCurrentDataProperty =
            DependencyProperty.Register("FilterCurrentData", typeof(FilterData), typeof(DataGridColumnFilter));

        public DataGridColumnHeader AssignedDataGridColumnHeader
        {
            get { return (DataGridColumnHeader)GetValue(AssignedDataGridColumnHeaderProperty); }
            set { SetValue(AssignedDataGridColumnHeaderProperty, value); }
        }

        public static readonly DependencyProperty AssignedDataGridColumnHeaderProperty =
            DependencyProperty.Register("AssignedDataGridColumnHeader", typeof(DataGridColumnHeader), typeof(DataGridColumnFilter));

        public DataGridColumn AssignedDataGridColumn
        {
            get { return (DataGridColumn)GetValue(AssignedDataGridColumnProperty); }
            set { SetValue(AssignedDataGridColumnProperty, value); }
        }

        public static readonly DependencyProperty AssignedDataGridColumnProperty =
            DependencyProperty.Register("AssignedDataGridColumn", typeof(DataGridColumn), typeof(DataGridColumnFilter));

        public DataGrid DataGrid
        {
            get { return (DataGrid)GetValue(DataGridProperty); }
            set { SetValue(DataGridProperty, value); }
        }

        public static readonly DependencyProperty DataGridProperty =
            DependencyProperty.Register("DataGrid", typeof(DataGrid), typeof(DataGridColumnFilter));

        public IEnumerable DataGridItemsSource
        {
            get { return (IEnumerable)GetValue(DataGridItemsSourceProperty); }
            set { SetValue(DataGridItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty DataGridItemsSourceProperty =
            DependencyProperty.Register("DataGridItemsSource", typeof(IEnumerable), typeof(DataGridColumnFilter));

        public bool IsFilteringInProgress
        {
            get { return (bool)GetValue(IsFilteringInProgressProperty); }
            set { SetValue(IsFilteringInProgressProperty, value); }
        }

        public static readonly DependencyProperty IsFilteringInProgressProperty =
            DependencyProperty.Register("IsFilteringInProgress", typeof(bool), typeof(DataGridColumnFilter));

        public FilterType FilterType { get { return FilterCurrentData != null ? FilterCurrentData.Type : FilterType.Text; } }

        public bool IsTextFilterControl
        {
            get { return (bool)GetValue(IsTextFilterControlProperty); }
            set { SetValue(IsTextFilterControlProperty, value); }
        }

        public static readonly DependencyProperty IsTextFilterControlProperty =
            DependencyProperty.Register("IsTextFilterControl", typeof(bool), typeof(DataGridColumnFilter));

        public bool IsTextContainsFilterControl
        {
            get { return (bool)GetValue(IsTextContainsFilterControlProperty); }
            set { SetValue(IsTextContainsFilterControlProperty, value); }
        }

        public static readonly DependencyProperty IsTextContainsFilterControlProperty =
            DependencyProperty.Register("IsTextContainsFilterControl", typeof(bool), typeof(DataGridColumnFilter));

        public bool IsNumericFilterControl
        {
            get { return (bool)GetValue(IsNumericFilterControlProperty); }
            set { SetValue(IsNumericFilterControlProperty, value); }
        }
        public static readonly DependencyProperty IsNumericFilterControlProperty =
            DependencyProperty.Register("IsNumericFilterControl", typeof(bool), typeof(DataGridColumnFilter));

        public bool IsNumericBetweenFilterControl
        {
            get { return (bool)GetValue(IsNumericBetweenFilterControlProperty); }
            set { SetValue(IsNumericBetweenFilterControlProperty, value); }
        }
        public static readonly DependencyProperty IsNumericBetweenFilterControlProperty =
            DependencyProperty.Register("IsNumericBetweenFilterControl", typeof(bool), typeof(DataGridColumnFilter));

        public bool IsBooleanFilterControl
        {
            get { return (bool)GetValue(IsBooleanFilterControlProperty); }
            set { SetValue(IsBooleanFilterControlProperty, value); }
        }
        public static readonly DependencyProperty IsBooleanFilterControlProperty =
            DependencyProperty.Register("IsBooleanFilterControl", typeof(bool), typeof(DataGridColumnFilter));

        public bool IsListFilterControl
        {
            get { return (bool)GetValue(IsListFilterControlProperty); }
            set { SetValue(IsListFilterControlProperty, value); }
        }
        public static readonly DependencyProperty IsListFilterControlProperty =
            DependencyProperty.Register("IsListFilterControl", typeof(bool), typeof(DataGridColumnFilter));

        public bool IsDateTimeFilterControl
        {
            get { return (bool)GetValue(IsDateTimeFilterControlProperty); }
            set { SetValue(IsDateTimeFilterControlProperty, value); }
        }
        public static readonly DependencyProperty IsDateTimeFilterControlProperty =
            DependencyProperty.Register("IsDateTimeFilterControl", typeof(bool), typeof(DataGridColumnFilter));

        public bool IsDateTimeBetweenFilterControl
        {
            get { return (bool)GetValue(IsDateTimeBetweenFilterControlProperty); }
            set { SetValue(IsDateTimeBetweenFilterControlProperty, value); }
        }
        public static readonly DependencyProperty IsDateTimeBetweenFilterControlProperty =
            DependencyProperty.Register("IsDateTimeBetweenFilterControl", typeof(bool), typeof(DataGridColumnFilter));

        public bool IsFirstFilterControl
        {
            get { return (bool)GetValue(IsFirstFilterControlProperty); }
            set { SetValue(IsFirstFilterControlProperty, value); }
        }
        public static readonly DependencyProperty IsFirstFilterControlProperty =
            DependencyProperty.Register("IsFirstFilterControl", typeof(bool), typeof(DataGridColumnFilter));

        public bool IsControlInitialized
        {
            get { return (bool)GetValue(IsControlInitializedProperty); }
            set { SetValue(IsControlInitializedProperty, value); }
        }
        public static readonly DependencyProperty IsControlInitializedProperty =
            DependencyProperty.Register("IsControlInitialized", typeof(bool), typeof(DataGridColumnFilter));
        #endregion

        #region Initialization
        private void Initialize()
        {
            if (DataGridItemsSource != null && AssignedDataGridColumn != null && DataGrid != null)
            {
                InitFilterData();

                InitControlType();

                HandleListFilterType();

                HookUpCommands();

                IsControlInitialized = true;
            }
        }

        private void InitFilterData()
        {
            if (FilterCurrentData == null || !FilterCurrentData.IsTypeInitialized)
            {
                string valuePropertyBindingPath = GetValuePropertyBindingPath(AssignedDataGridColumn);

                bool typeInitialized;

                Type valuePropertyType = GetValuePropertyType(
                    valuePropertyBindingPath, GetItemSourceElementType(out typeInitialized));

                FilterType filterType = GetFilterType(
                    valuePropertyType, 
                    IsComboDataGridColumn(),
                    IsBetweenType(),
                    IsTextContainsColumn());

                var filterOperator = FilterOperator.Undefined;

                string queryString   = String.Empty;
                string queryStringTo = String.Empty;

                FilterCurrentData = new FilterData(
                    filterOperator, 
                    filterType, 
                    valuePropertyBindingPath, 
                    valuePropertyType, 
                    queryString, 
                    queryStringTo,
                    typeInitialized,
                    DataGridColumnExtensions.GetIsCaseSensitiveSearch(AssignedDataGridColumn));
            }
        }

        private void InitControlType()
        {
            IsFirstFilterControl    = false;

            IsTextFilterControl     = false;
            IsNumericFilterControl  = false;
            IsBooleanFilterControl  = false;
            IsListFilterControl     = false;
            IsDateTimeFilterControl = false;

            IsNumericBetweenFilterControl = false;
            IsDateTimeBetweenFilterControl = false;
            IsTextContainsFilterControl = false;

            if (FilterType == FilterType.Text)
            {
                IsTextFilterControl = true;
            }
            else if (FilterType == FilterType.Numeric)
            {
                IsNumericFilterControl = true;
            }
            else if (FilterType == FilterType.Boolean)
            {
                IsBooleanFilterControl = true;
            }
            else if (FilterType == FilterType.List)
            {
                IsListFilterControl = true;
            }
            else if (FilterType == FilterType.DateTime)
            {
                IsDateTimeFilterControl = true;
            }
            else if (FilterType == FilterType.NumericBetween)
            {
                IsNumericBetweenFilterControl = true;
            }
            else if (FilterType == FilterType.DateTimeBetween)
            {
                IsDateTimeBetweenFilterControl = true;
            }
            else if (FilterType == FilterType.TextContains)
            {
                IsTextContainsFilterControl = true;
            }
        }

        private void HandleListFilterType()
        {
            if (FilterCurrentData.Type == FilterType.List)
            {
                ComboBox comboBox             = this.Template.FindName("PART_ComboBoxFilter", this) as ComboBox;
                DataGridComboBoxColumn column = AssignedDataGridColumn as DataGridComboBoxColumn;

                if (comboBox != null && column != null)
                {

                    if (DataGridComboBoxExtensions.GetIsTextFilter(column))
                    {
                        FilterCurrentData.Type = FilterType.Text;
                        InitControlType();
                    }
                    else //list filter type
                    {
                        Binding columnItemsSourceBinding = BindingOperations.GetBinding(column, DataGridComboBoxColumn.ItemsSourceProperty);

                        if (columnItemsSourceBinding == null)
                        {
                            Setter styleSetter = column
                                .EditingElementStyle
                                .Setters
                                .FirstOrDefault(s => ((Setter)s).Property == DataGridComboBoxColumn.ItemsSourceProperty) as Setter;
                            if (styleSetter != null)
                                columnItemsSourceBinding = styleSetter.Value as Binding;
                        }

                        comboBox.DisplayMemberPath = column.DisplayMemberPath;
                        comboBox.SelectedValuePath = column.SelectedValuePath;

                        if (columnItemsSourceBinding != null)
                        {
                            BindingOperations.SetBinding(comboBox, ItemsControl.ItemsSourceProperty, columnItemsSourceBinding);
                        }

                        comboBox.RequestBringIntoView += SetComboBindingAndHanldeUnsetValue;
                    }
                }
            }
        }

        private void SetComboBindingAndHanldeUnsetValue(object sender, RequestBringIntoViewEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            DataGridComboBoxColumn column = AssignedDataGridColumn as DataGridComboBoxColumn;

            if (column.ItemsSource == null)
            {
                if (combo.ItemsSource != null)
                {
                    IList list = combo.ItemsSource.Cast<object>().ToList();

                    if (list.Count > 0 && list[0] != DependencyProperty.UnsetValue)
                    {
                        combo.RequestBringIntoView -= SetComboBindingAndHanldeUnsetValue;

                        list.Insert(0, DependencyProperty.UnsetValue);

                        combo.DisplayMemberPath = column.DisplayMemberPath;
                        combo.SelectedValuePath = column.SelectedValuePath;

                        combo.ItemsSource = list;
                    }
                }
            }
            else
            {
                combo.RequestBringIntoView -= SetComboBindingAndHanldeUnsetValue;

                IList comboList = null;
                IList columnList = null;

                if (combo.ItemsSource != null)
                {
                    comboList = combo.ItemsSource.Cast<object>().ToList();
                }

                columnList = column.ItemsSource.Cast<object>().ToList();

                if (comboList == null ||
                    (columnList.Count > 0 && columnList.Count + 1 != comboList.Count))
                {
                    columnList = column.ItemsSource.Cast<object>().ToList();
                    columnList.Insert(0, DependencyProperty.UnsetValue);

                    combo.ItemsSource = columnList;
                }

                combo.RequestBringIntoView += SetComboBindingAndHanldeUnsetValue;
            }
        }

        private string GetValuePropertyBindingPath(DataGridColumn column)
        {
            string path = DataGridColumnExtensions.GetFilterMemberPathProperty(column);

            if (path == null)
            {
                path = String.Empty;

                if (column is DataGridBoundColumn)
                {
                    DataGridBoundColumn bc = column as DataGridBoundColumn;
                    path = (bc.Binding as Binding).Path.Path;
                }
                else if (column is DataGridTemplateColumn)
                {
                    DataGridTemplateColumn tc = column as DataGridTemplateColumn;

                    object templateContent = tc.CellTemplate.LoadContent();

                    if (templateContent != null && templateContent is TextBlock)
                    {
                        TextBlock block = templateContent as TextBlock;

                        BindingExpression binding = block.GetBindingExpression(TextBlock.TextProperty);

                        path = binding.ParentBinding.Path.Path;
                    }
                    else if (templateContent != null && templateContent is Button)
                    {
                        Button block = templateContent as Button;

                        BindingExpression binding = block.GetBindingExpression(Button.ContentProperty);

                        path = binding.ParentBinding.Path.Path;
                    }
                }
                else if (column is DataGridComboBoxColumn)
                {
                    DataGridComboBoxColumn comboColumn = column as DataGridComboBoxColumn;

                    path = null;

                    Binding binding = ((comboColumn.SelectedValueBinding) as Binding);

                    if (binding == null)
                    {
                        binding = ((comboColumn.SelectedItemBinding) as Binding);
                    }

                    if (binding == null)
                    {
                        binding = comboColumn.SelectedValueBinding as Binding;
                    }

                    if (binding != null)
                    {
                        path = binding.Path.Path;
                    }

                    if (comboColumn.SelectedItemBinding != null && comboColumn.SelectedValueBinding == null)
                    {
                        if (path != null && path.Trim().Length > 0)
                        {
                            if (DataGridComboBoxExtensions.GetIsTextFilter(comboColumn))
                            {
                                path += "." + comboColumn.DisplayMemberPath;
                            }
                            else
                            {
                                path += "." + comboColumn.SelectedValuePath;
                            }
                        }
                    }
                }
            }

            return path;
        }

        private Type GetValuePropertyType(string path, Type elementType)
        {
            Type type = typeof(object);

            if (elementType != null)
            {
                string[] properties = path.Split(".".ToCharArray()[0]);

                PropertyInfo pi = null;

                if (properties.Length == 1)
                {
                    pi = elementType.GetProperty(path);
                }
                else
                {
                    pi = elementType.GetProperty(properties[0]);

                    for (int i = 1; i < properties.Length; i++)
                    {
                        if (pi != null)
                        {
                            pi = pi.PropertyType.GetProperty(properties[i]);
                        }
                    }
                }


                if (pi != null)
                {
                    type = pi.PropertyType;
                }
            }

            return type;
        }

        private Type GetItemSourceElementType(out bool typeInitialized)
        {
            typeInitialized = false;

            Type elementType = null;

            IList l = (DataGridItemsSource as IList);

            if (l != null && l.Count > 0)
            {
                object obj = l[0];

                if (obj != null)
                {
                    elementType = l[0].GetType();
                    typeInitialized = true;
                }
                else
                {
                    elementType = typeof(object);
                }
            }
            if (l == null)
            {
                ListCollectionView lw = (DataGridItemsSource as ListCollectionView);

                if (lw != null && lw.Count > 0)
                {
                    object obj = lw.SourceCollection.Cast<object>().FirstOrDefault();

                    if (obj != null)
                    {
                        elementType = obj.GetType();
                        typeInitialized = true;
                    }
                    else
                    {
                        elementType = typeof(object);
                    }
                }
            }

            return elementType;
        }

        private FilterType GetFilterType(
            Type valuePropertyType, 
            bool isAssignedDataGridColumnComboDataGridColumn,
            bool isBetweenType,
            bool isTextContains)
        {
            FilterType filterType;

            if (isAssignedDataGridColumnComboDataGridColumn)
            {
                filterType = FilterType.List;
            }
            else if (valuePropertyType == typeof(Boolean) || valuePropertyType == typeof(bool?))
            {
                filterType = FilterType.Boolean;
            }
            else if (valuePropertyType == typeof(SByte) || valuePropertyType == typeof(sbyte?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(Byte) || valuePropertyType == typeof(byte?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(Int16) || valuePropertyType == typeof(short?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(UInt16) || valuePropertyType == typeof(ushort?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(Int32) || valuePropertyType == typeof(int?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(UInt32) || valuePropertyType == typeof(uint?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(Int64) || valuePropertyType == typeof(long?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(Single) || valuePropertyType == typeof(float?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(Int64) || valuePropertyType == typeof(long?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(Decimal) || valuePropertyType == typeof(decimal?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(float) || valuePropertyType == typeof(float?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(Double) || valuePropertyType == typeof(double?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(Int64) || valuePropertyType == typeof(long?))
            {
                filterType = FilterType.Numeric;
            }
            else if (valuePropertyType == typeof(DateTime) || valuePropertyType == typeof(DateTime?))
            {
                filterType = FilterType.DateTime;
            }
            else
            {
                filterType = FilterType.Text;
            }

            if (filterType == FilterType.Numeric && isBetweenType)
            {
                filterType = FilterType.NumericBetween;
            }
            else if (filterType == FilterType.DateTime && isBetweenType)
            {
                filterType = FilterType.DateTimeBetween;
            }
            else if (filterType == FilterType.Text && isTextContains)
            {
                filterType = FilterType.TextContains;
            }

            return filterType;
        }

        private bool IsComboDataGridColumn()
        {
            return AssignedDataGridColumn is DataGridComboBoxColumn;
        }

        private bool IsBetweenType()
        {
            return DataGridColumnExtensions.GetIsBetweenFilterControl(AssignedDataGridColumn);
        }

        private bool IsTextContainsColumn()
        {
            return DataGridColumnExtensions.GetContainsSearchProperty(AssignedDataGridColumn);
        }

        private void HookUpCommands()
        {
            if (DataGridExtensions.GetClearFilterCommand(DataGrid) == null)
            {
                DataGridExtensions.SetClearFilterCommand(
                    DataGrid, new DataGridFilterCommand(ClearQuery));
            }
        }
        #endregion

        #region Querying
        void filterCurrentData_FilterChangedEvent(object sender, EventArgs e)
        {
            if (DataGrid != null)
            {
                QueryController query = QueryControllerFactory.GetQueryController(
                    DataGrid, FilterCurrentData, DataGridItemsSource);

                AddFilterStateHandlers(query);

                query.DoQuery();

                IsFirstFilterControl = query.IsCurentControlFirstControl;
            }
        }

        private void ClearQuery(object parameter)
        {
            if (DataGrid != null)
            {
                QueryController query = QueryControllerFactory.GetQueryController(
                    DataGrid, FilterCurrentData, DataGridItemsSource);

                query.ClearFilter();
            }
        }

        private void AddFilterStateHandlers(QueryController query)
        {
            query.FilteringStarted -= query_FilteringStarted;
            query.FilteringFinished -=query_FilteringFinished;

            query.FilteringStarted += query_FilteringStarted;
            query.FilteringFinished +=query_FilteringFinished;
        }

        void query_FilteringFinished(object sender, EventArgs e)
        {
            if (FilterCurrentData.Equals((sender as QueryController).ColumnFilterData))
            {
                this.IsFilteringInProgress = false;
            }
        }

        void query_FilteringStarted(object sender, EventArgs e)
        {
            if (FilterCurrentData.Equals((sender as QueryController).ColumnFilterData))
            {
                this.IsFilteringInProgress = true;
            }
        }
        #endregion
    }
}