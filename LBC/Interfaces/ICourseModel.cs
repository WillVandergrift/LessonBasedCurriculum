using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lbcLibrary;
using System.Collections.ObjectModel;
using LBC.Helper;

namespace LBC.Interfaces
{
    interface ICourseModel
    {
        Course CreateCourse(ref ObservableCollection<CourseMeta> crsMeta);
        bool DeleteCourse(Course crs);
    }
}
