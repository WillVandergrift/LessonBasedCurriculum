using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using lbcLibrary;
using System.ComponentModel;

namespace LBC.Helper
{
    class TreeCourseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<Course> courses = value as IEnumerable<Course>;
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(courses);
            lcv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            return lcv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
