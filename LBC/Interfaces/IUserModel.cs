using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lbcLibrary;
using System.Collections.ObjectModel;
using LBC.Helper;

namespace LBC.Interfaces
{
    interface IUserModel
    {
        LbcUser LoadUserByCredentials(string userName, string password);
        void UpdateUserLastLoginDate(LbcUser user, DateTime lastLogin);
        ObservableCollection<Course> LoadUserCurriculum(LbcUser user, ref ObservableCollection<CoursePermission> coursePermissions, ref ObservableCollection<CourseMeta> metaData);
        ObservableCollection<LbcUser> GetAllUsers();
    }
}
