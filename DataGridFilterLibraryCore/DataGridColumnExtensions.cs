using System.Windows;
using System.Windows.Controls;

namespace DataGridFilterLibrary
{
    public class DataGridColumnExtensions
    {
        //case sensitive
        public static DependencyProperty IsCaseSensitiveSearchProperty =
            DependencyProperty.RegisterAttached("IsCaseSensitiveSearch",
                typeof(bool), typeof(DataGridColumn));

        public static bool GetIsCaseSensitiveSearch(DependencyObject target)
        {
            return (bool)target.GetValue(IsCaseSensitiveSearchProperty);
        }

        public static void SetIsCaseSensitiveSearch(DependencyObject target, bool value)
        {
            target.SetValue(IsCaseSensitiveSearchProperty, value);
        }

        //between
        public static DependencyProperty IsBetweenFilterControlProperty =
            DependencyProperty.RegisterAttached("IsBetweenFilterControl",
                typeof(bool), typeof(DataGridColumn));

        public static bool GetIsBetweenFilterControl(DependencyObject target)
        {
            return (bool)target.GetValue(IsBetweenFilterControlProperty);
        }

        public static void SetIsBetweenFilterControl(DependencyObject target, bool value)
        {
            target.SetValue(IsBetweenFilterControlProperty, value);
        }

        //don't generate filter control
        public static DependencyProperty DoNotGenerateFilterControlProperty =
            DependencyProperty.RegisterAttached("DoNotGenerateFilterControl",
                typeof(bool), typeof(DataGridColumn), new PropertyMetadata(false));

        public static bool GetDoNotGenerateFilterControl(DependencyObject target)
        {
            return (bool)target.GetValue(DoNotGenerateFilterControlProperty);
        }

        public static void SetDoNotGenerateFilterControl(DependencyObject target, bool value)
        {
            target.SetValue(DoNotGenerateFilterControlProperty, value);
        }

        //filter member path
        public static DependencyProperty FilterMemberPathProperty =
            DependencyProperty.RegisterAttached("FilterMemberPath",
                typeof(string), typeof(DataGridColumn), new PropertyMetadata(null));

        public static string GetFilterMemberPathProperty(DependencyObject target)
        {
            return (string)target.GetValue(FilterMemberPathProperty);
        }

        public static void SetFilterMemberPathProperty(DependencyObject target, string value)
        {
            target.SetValue(FilterMemberPathProperty, value);
        }

        //Contains
        public static DependencyProperty ContainsSearchProperty =
            DependencyProperty.RegisterAttached("ContainsSearch",
                typeof(bool), typeof(DataGridColumn));

        public static bool GetContainsSearchProperty(DependencyObject target)
        {
            return (bool)target.GetValue(ContainsSearchProperty);
        }

        public static void SetContainsSearchProperty(DependencyObject target, bool value)
        {
            target.SetValue(ContainsSearchProperty, value);
        }
    }
}
