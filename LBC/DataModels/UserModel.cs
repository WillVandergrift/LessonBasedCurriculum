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
    class UserModel : IUserModel
    {
        /// <summary>
        /// Returns a collection of all users in the database
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<LbcUser> GetAllUsers()
        {
            ObservableCollection<LbcUser> districtUsers = new ObservableCollection<LbcUser>(Session.DbContext.LbcUsers.ToList());

            return districtUsers;
        }


        /// <summary>
        /// Return the user object with the given login credentials
        /// </summary>
        /// <param name="userName">The user's login name</param>
        /// <param name="password">The user's password</param>
        /// <returns></returns>
        public LbcUser LoadUserByCredentials(string userName, string password)
        {
            IQueryable<LbcUser> matchingUsers = Session.DbContext.LbcUsers.Where(u => u.UserName == userName && u.UserPassword == password);

            if (matchingUsers.Count() > 0)
            {
                return matchingUsers.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Update the user's last login date and time
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <param name="lastLogin">The last login date for the user</param>
        public void UpdateUserLastLoginDate(LbcUser user, DateTime lastLogin)
        {
            user.LastLogin = lastLogin;
        }
    
        /// <summary>
        /// Load the user's curriculum
        /// </summary>
        /// <param name="user">The user to load curriculum for</param>
        /// <returns></returns>
        public ObservableCollection<Course> LoadUserCurriculum(LbcUser user, ref ObservableCollection<CoursePermission> coursePermissions, ref ObservableCollection<CourseMeta> metaData)
        {
            ObservableCollection<Course> userCourses = new ObservableCollection<Course>();
            List<CoursePermission> userPermissions;
            metaData = new ObservableCollection<CourseMeta>();
            coursePermissions = new ObservableCollection<CoursePermission>();
            CourseMeta tmpMeta = new CourseMeta();

            try
            {
                userPermissions = Session.DbContext.CoursePermissions.Where(cp => cp.LbcUser == user).ToList();

                foreach (CoursePermission cp in userPermissions)
                {
                    tmpMeta = new CourseMeta();

                    userCourses.Add(cp.Course);
                    coursePermissions.Add(cp);
                    tmpMeta.CourseID = cp.CourseID;
                    tmpMeta.Permission = cp.Permission;
                    metaData.Add(tmpMeta);
                }

                return userCourses;
            }
            catch
            {
                //An error occured while loading the curriculum
                return null;
            }
            finally
            {
                //Clean up local variables
                userCourses = null;
                userPermissions = null;
            }

        }
    }
}
