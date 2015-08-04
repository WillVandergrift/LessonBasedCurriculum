using System.Collections.ObjectModel;
using lbcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBC.Interfaces;
using LBC.Helper;

namespace LBC.DataModels
{
    class ProcessStandardModel : IProcessStandardModel
    {
        public ObservableCollection<ProcessStandard> LoadAllProcessStandards()
        {
            ObservableCollection<ProcessStandard> allStandards = new ObservableCollection<ProcessStandard>();
            IQueryable<ProcessStandard> loadedStandards;

            try
            {
                loadedStandards = Session.DbContext.ProcessStandards;

                foreach (ProcessStandard ps in loadedStandards)
                {
                    allStandards.Add(ps);
                }

                return allStandards;
            }
            catch
            {
                return null;
            }
            finally
            {
                allStandards = null;
                loadedStandards = null;
            }
        }
    }
}
