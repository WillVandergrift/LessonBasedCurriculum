using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using lbcLibrary;
using System.Collections.ObjectModel;

namespace LBC.Helper
{
    class Information
    {
        // =======================================================
        // Events
        // =======================================================
        public event PropertyChangedEventHandler PropertyChanged;

        // ===========================================
        // Private Variables
        // ===========================================
        private ImageSource _Icon;
        private string _Header;
        private string _Message;
        private string _InformationType;
        private string _Tag;
        private object _MetaData;


        // ===========================================
        // Public Properties
        // ===========================================
        public ImageSource Icon
        {
            get
            {
                return _Icon;
            }
            set
            {
                if (value != null)
                {
                    _Icon = value;
                }
            }
        }
        public string Header
        {
            get
            {
                return _Header;
            }
            set
            {
                if (value != null)
                {
                    _Header = value;
                }
            }
        }
        /// <summary>
        /// The specific type of information
        /// </summary>
        public string InformationType
        {
            get
            {
                return _InformationType;
            }
            set
            {
                _InformationType = value;
            }
        }
        /// <summary>
        /// An object that is associted with the information
        /// </summary>
        public object MetaData
        {
            get
            {
                return _MetaData;
            }
            set
            {
                _MetaData = value;
            }
        }
        /// <summary>
        /// A string that is associated with the information
        /// </summary>
        public string Tag
        {
            get
            {
                return _Tag;
            }
            set
            {
                _Tag = value;
            }
        }
        /// <summary>
        /// The main content of the information message
        /// </summary>
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                if (value != null)
                {
                    _Message = value;
                }
            }
        }

        // ===========================================
        // Methods
        // ===========================================
        public Information(string icon, string header, string message)
        {
            Icon = GetIcon(icon);
            Header = header;
            Message = message;
        }
        public Information(string icon, string header, string message, string type, object meta, string tag)
        {
            Icon = GetIcon(icon);
            Header = header;
            Message = message;
            InformationType = type;
            MetaData = meta;
            Tag = tag;
        }

        public void UpdateIcon(string icon)
        {
            Icon = GetIcon(icon);
        }

        private BitmapImage GetIcon(string iconType)
        {
            switch (iconType)
            {
                case "Warning":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Warning-64.png", UriKind.Relative));
                case "Login Success":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/User-48.png", UriKind.Relative));
                case "Info":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Info-64.png", UriKind.Relative));
                case "Download":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Download-64.png", UriKind.Relative));
                case "DownloadDocument":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/DownloadDocument-48.png", UriKind.Relative));
                case "DownloadComplete":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Check-64.png", UriKind.Relative));
                case "UploadDocument":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/UploadDocument-48.png", UriKind.Relative));
                case "DeleteDocument":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/DeleteDocument-64.png", UriKind.Relative));
                case "DeleteExpectation":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/DeleteExpectation-64.png", UriKind.Relative));
                case "UploadComplete":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Check-64.png", UriKind.Relative));
                case "LBC":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/lbc-64.png", UriKind.Relative));
                case "Save":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Save-64.png", UriKind.Relative));
                case "Unit":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Unit-64.png", UriKind.Relative));
                case "UnitDelete":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Delete-Unit-64.png", UriKind.Relative));
                case "MoveUnit":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Move-Unit-64.png", UriKind.Relative));
                case "Lesson":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Lesson-64.png", UriKind.Relative));
                case "Course":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Course-64.png", UriKind.Relative));
                case "CourseDelete":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Delete-Course-64.png", UriKind.Relative));
                case "LessonDelete":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Delete-Lesson-64.png", UriKind.Relative));
                case "MoveLesson":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Move-Lesson-64.png", UriKind.Relative));
                case "Settings":
                    return new BitmapImage(new Uri(@"/LBC;component/Images/Settings-64.png", UriKind.Relative));
                default:
                    return null;
            }
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

        // ===============================================
        // Static Methods
        // ===============================================

        /// <summary>
        /// Create an Information message greeting the current user
        /// </summary>
        /// <param name="usr"></param>
        /// <returns></returns>
        public static Information UserGreeting(LbcUser usr)
        {           
            string _WelcomeHeader = "Welcome Back, " + usr.FirstName;
            string _WelcomeMessage;

            if (usr.LastLogin != null)
            {
                DateTime _LastLogin = usr.LastLogin.Value;
                _WelcomeMessage = "Your last login date was: " + _LastLogin.ToString("f");
            }
            else
            {
                _WelcomeMessage = "It appears this is your first time logging in.";
            }

            return new Information("Login Success",_WelcomeHeader,_WelcomeMessage);
        }
    }
}
