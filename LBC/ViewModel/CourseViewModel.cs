using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LBC.Helper;
using lbcLibrary;
using System.Collections.ObjectModel;
using LBC.Interfaces;
using LBC.DataModels;

namespace LBC.ViewModel
{
    class CourseViewModel
    {
        // =======================================================
        // Events
        // =======================================================
        public event PropertyChangedEventHandler PropertyChanged;

        // =======================================================
        // Private Variables
        // =======================================================
        private Course _CurCourse;
        private CourseMeta _CurCourseMeta;
        private ICourseModel crsContext = new CourseModel();

        // =======================================================
        // Public Properties
        // =======================================================
        public Course CurCourse
        {
            get
            {
                return _CurCourse;
            }
            set
            {
                if (value != null)
                {
                    _CurCourse = value;
                    NotifyPropertyChanged("CurCourse");
                }
            }
        }
        public CourseMeta CurCourseMeta
        {
            get
            {
                return _CurCourseMeta;
            }
            set
            {
                if (value != null)
                {
                    _CurCourseMeta = value;
                    NotifyPropertyChanged("CurCourseMeta");
                }
            }
        }

        public Course CreateCourse(ref ObservableCollection<CourseMeta> crsMeta)
        {
            return crsContext.CreateCourse(ref crsMeta);
        }

        /// <summary>
        /// Set CurCourseMeta for the object that matches CurCourse
        /// </summary>
        /// <param name="curCourse"></param>
        public void UpdateCourseMeta(ObservableCollection<CourseMeta> metaData)
        {
            CurCourseMeta = metaData.Where(c => c.CourseID == _CurCourse.ID).First();  
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
