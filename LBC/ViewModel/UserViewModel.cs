using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using lbcLibrary;
using LBC.Interfaces;
using LBC.DataModels;
using System.Collections.ObjectModel;
using LBC.Helper;

namespace LBC.ViewModel
{
    class UserViewModel
    {
        // =======================================================
        // Events
        // =======================================================
        public event PropertyChangedEventHandler PropertyChanged;

        // =======================================================
        // Private Variables
        // =======================================================
        private LbcUser _CurUser;
        private ObservableCollection<LbcUser> _Users;
        private IUserModel usrContext = new UserModel();


        // =======================================================
        // Public Properties
        // =======================================================
        public LbcUser CurUser
        {
            get
            {
                return _CurUser;
            }
            set
            {
                if (value != null)
                {
                    _CurUser = value;
                    NotifyPropertyChanged("CurUser");
                }
            }
        }
        public ObservableCollection<LbcUser> Users
        {
            get
            {
                return _Users;
            }
            set
            {
                if (value != null)
                {
                    _Users = value;
                    NotifyPropertyChanged("Users");
                }
            }
        }

        // =======================================================
        // Methods
        // =======================================================

        /// <summary>
        /// Attempt to log the user in with the given credentials
        /// </summary>
        /// <param name="User">The user's login name</param>
        /// <param name="Pass">The user's password</param>
        /// <returns></returns>
        public LbcUser AuthenticateUser(string User, string Pass)
        {
            try
            {
                return usrContext.LoadUserByCredentials(User, Pass);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Load the given user's curriculum
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ObservableCollection<Course> LoadCurriculum(LbcUser user, ref ObservableCollection<CoursePermission> permissions, ref ObservableCollection<CourseMeta> meta)
        {
            return usrContext.LoadUserCurriculum(user, ref permissions, ref meta);
        }

        /// <summary>
        /// Loads a list of all user objects into _Users
        /// </summary>
        /// <returns></returns>
        public bool LoadDistrictUserList()
        {
            try
            {
                _Users = usrContext.GetAllUsers();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update the user's last login date and time
        /// </summary>
        public void UpdateUserLastLogin()
        {
            usrContext.UpdateUserLastLoginDate(Helper.Session.CurUser, DateTime.Now);
            Session.DbContext.SaveChanges();
        }

        /// <summary>
        /// Notify the UI that the specified property has changed
        /// </summary>
        /// <param name="item"></param>
        public void NotifyPropertyChanged(string item)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(item));
            }
        }
    }
}
