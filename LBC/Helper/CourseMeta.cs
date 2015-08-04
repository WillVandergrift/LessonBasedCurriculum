using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using lbcLibrary;
using System.ComponentModel;

namespace LBC.Helper
{
    class CourseMeta
    {
        // =======================================================
        // Events
        // =======================================================
        public event PropertyChangedEventHandler PropertyChanged;

        private Guid _CourseID;
        private string _Permission;
        private ObservableCollection<Expectation> _CourseExpectations = new ObservableCollection<Expectation>();

        /// <summary>
        /// The ID for the matching Course object
        /// </summary>
        public Guid CourseID
        {
            get
            {
                return _CourseID;
            }
            set
            {
                if (value != null)
                {
                    _CourseID = value;
                    NotifyPropertyChanged("CourseID");
                }
            }
        }

        /// <summary>
        /// The user's permission level for the matching course
        /// </summary>
        public String Permission
        {
            get
            {
                return _Permission;
            }
            set
            {
                if (value != null)
                {
                    _Permission = value;
                    NotifyPropertyChanged("Permission");
                }
            }
        }

        /// <summary>
        /// A list of expectations for the given course
        /// </summary>
        public ObservableCollection<Expectation> CourseExpectations
        {
            get
            {
                return _CourseExpectations;
            }
            set
            {
                if (value != null)
                {
                    _CourseExpectations = value;
                    NotifyPropertyChanged("CourseExpectations");
                }
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
    }
}
