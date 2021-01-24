DataGridFilterCore
==============

<img alt="Nuget" src="https://img.shields.io/nuget/v/DataGridFilterCore">

A component for easy filtering of WPF DataGrids. Forked from the project originally posted by Sanjin Matusan [here](http://www.codeproject.com/Articles/42227/Automatic-WPF-Toolkit-DataGrid-Filtering).

**Usage**

Just drop it in and you're good to go.

Behavior for the entire Datagrid can be modified using the DataGridExtensions:

 * DataGridFilterQueryController
 * ClearFilterCommand
 * IsFilterVisible
 * UseBackgroundWorkerForFiltering
 * IsClearButtonVisible

For example:

```
<Window xmlns:filterLibrary="clr-namespace:DataGridFilterLibrary;assembly=DataGridFilterCore">
<DataGrid filterLibrary:DataGridExtensions.UseBackgroundWorkerForFiltering="True" ...
```

Behavior can be modified per-column by using the DataGridColumnExtensions:

 * IsCaseSensitiveSearch
 * IsBetweenFilterControl
 * DoNotGenerateFilterControl
 * FilterMemberPath
 * ContainsSearch

or DataGridComboBoxExtensions:

 * IsTextFilter
 * UserCanEnterText

For example:

```
<DataGridTextColumn filterLibrary:DataGridColumnExtensions.DoNotGenerateFilterControl="True" ...
```											