using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using lbcLibrary;
using System.ComponentModel;
using System.Globalization;

namespace LBC.Helper
{
    class TreeUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<Unit> units = value as IEnumerable<Unit>;
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(units);
            lcv.SortDescriptions.Add(new SortDescription("UnitNumber", ListSortDirection.Ascending));
            return lcv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
