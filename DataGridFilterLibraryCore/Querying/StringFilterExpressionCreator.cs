using System;
using System.Collections.Generic;
using System.Text;
using DataGridFilterLibrary.Support;

namespace DataGridFilterLibrary.Querying
{
    internal class StringFilterExpressionCreator
    {
        const string WildcardAnyString = "%";

        private enum StringExpressionFunction
        {
            Undefined = 0,
            StartsWith = 1,
            IndexOf = 2,
            EndsWith = 3
        }

        private readonly FilterData _filterData;
        private readonly List<object> _paramseters;
        private readonly ParameterCounter _paramCounter;

        internal int ParametarsCrated { get { return _paramseters.Count; } }

        internal StringFilterExpressionCreator(
            ParameterCounter paramCounter, FilterData filterData, List<object> paramseters)
        {
            _paramCounter = paramCounter;
            _filterData = filterData;
            _paramseters = paramseters;
        }

        internal string Create()
        {
            StringBuilder filter = new StringBuilder();
            
            List<string> filterList = Parse(_filterData.QueryString);

            for (int i = 0; i < filterList.Count; i++)
            {
                if (i > 0) filter.Append(" and ");

                filter.Append(filterList[i]);
            }

            return filter.ToString();
        }

        private List<string> Parse(string filterString)
        {
            if (_filterData.Type == FilterType.TextContains)
            {
                filterString = string.Format("{0}{1}{0}", WildcardAnyString, filterString);
            }

            string token = null;
            int i = 0;
            bool expressionCompleted = false;
            List<string> filter = new List<string>();
            string expressionValue = String.Empty;
            StringExpressionFunction function = StringExpressionFunction.Undefined;

            do
            {
                token = i < filterString.Length ? filterString[i].ToString() : null;

                if (token == WildcardAnyString || token == null)
                {
                    if (expressionValue.StartsWith(WildcardAnyString) && token != null)
                    {
                        function = StringExpressionFunction.IndexOf;

                        expressionCompleted = true;
                    }
                    else if (expressionValue.StartsWith(WildcardAnyString) && token == null)
                    {
                        function = StringExpressionFunction.EndsWith;

                        expressionCompleted = false;
                    }
                    else
                    {
                        function = StringExpressionFunction.StartsWith;

                        if (filterString.Length - 1 > i) expressionCompleted = true;
                    }
                }

                if (token == null)
                {
                    expressionCompleted = true;
                }

                expressionValue += token;

                if (expressionCompleted
                    && function != StringExpressionFunction.Undefined
                    && expressionValue != String.Empty)
                {
                    string expressionValueCopy = String.Copy(expressionValue);

                    expressionValueCopy = expressionValueCopy.Replace(WildcardAnyString, String.Empty);

                    if (expressionValueCopy != String.Empty)
                    {
                        filter.Add(CreateFunction(function, expressionValueCopy));
                    }

                    function = StringExpressionFunction.Undefined;

                    expressionValue = expressionValue.EndsWith(WildcardAnyString) ? WildcardAnyString : String.Empty;

                    expressionCompleted = false;
                }

                i++;

            } while (token != null);

            return filter;
        }

        private string CreateFunction(
            StringExpressionFunction function, string value)
        {
            StringBuilder filter = new StringBuilder();

            _paramseters.Add(value);

            filter.Append(_filterData.ValuePropertyBindingPath);

            if (_filterData.ValuePropertyType.IsGenericType
                && _filterData.ValuePropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                filter.Append(".Value");
            }

            _paramCounter.Increment();
            _paramCounter.Increment();

            filter.AppendFormat(".ToString().{0}(@{1}, @{2})", function, (_paramCounter.ParameterNumber - 1), (_paramCounter.ParameterNumber));

            if (function == StringExpressionFunction.IndexOf)
            {
                filter.Append(" != -1 ");
            }

            _paramseters.Add(_filterData.IsCaseSensitiveSearch 
                ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);

            return filter.ToString();
        }
    }
}
