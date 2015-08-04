using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lbcLibrary;
using System.Collections.ObjectModel;

namespace LBC.Interfaces
{
    interface IProcessStandardModel
    {
        ObservableCollection<ProcessStandard> LoadAllProcessStandards();
    }
}
