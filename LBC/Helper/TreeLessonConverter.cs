using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lbcLibrary;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace LBC.Helper
{
    class TreeLessonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<Lesson> lessons = value as IEnumerable<Lesson>;
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(lessons);
            lcv.SortDescriptions.Add(new SortDescription("LessonNum", ListSortDirection.Ascending));
            return lcv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
