using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace LBC.Helper
{
    class GUI
    {
        public static bool RibbonHasFocus(DependencyObject selectedObject)
        {
            try
            {
                //True if the ribbon bar or a child element of the ribbon is selected
                if (ItemInVisualTree("RibbonBar", selectedObject))
                {
                    return true;
                }
                //True if the ribbon color picker is selected
                else if (ItemOfType("ToggleButton", selectedObject))
                {
                    return true;
                }
                //True if the ribbon font selector or font size selected is selected
                else if (ItemOfType("PickerToggleButton", selectedObject))
                {
                    return true;
                }
                //True if the ribbon font selector or font size selected is selected
                else if (ItemOfType("RadRibbonComboBox", selectedObject))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the ItemType was found in the visual tree of the obj
        /// </summary>
        /// <param name="ItemType">The type to look for</param>
        /// <param name="obj">The item to inspect</param>
        /// <returns></returns>
        private static bool ItemInVisualTree(string ItemType, DependencyObject obj)
        {
            String ItemToSearch = VisualTreeHelper.GetParent(obj).GetType().ToString();

            List<String> ItemToSearchParts = new List<string>();
            ItemToSearchParts.AddRange(ItemToSearch.Split('.'));

            foreach (String item in ItemToSearchParts)
            {
                if (item == ItemType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the Item is of the specified type or derived from the specified type
        /// </summary>
        /// <param name="ItemType">The type to look for</param>
        /// <param name="obj">The item to inspect</param>
        /// <returns></returns>
        private static bool ItemOfType(string ItemType, Object obj)
        {
            String ItemToSearch = obj.GetType().ToString();

            List<String> ItemToSearchParts = new List<string>();
            ItemToSearchParts.AddRange(ItemToSearch.Split('.'));

            foreach (String item in ItemToSearchParts)
            {
                if (item == ItemType)
                {
                    return true;
                }
            }

            return false;
        }


    }
}
