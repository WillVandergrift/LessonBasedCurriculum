using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LBC.Helper;
using System.Collections.ObjectModel;
using LBC.Interfaces;
using lbcLibrary;
using acManagement;
using LBC.DataModels;

namespace LBC.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        // =======================================================
        // Events
        // =======================================================
        public event PropertyChangedEventHandler PropertyChanged;

        // =======================================================
        // Private Variables
        // =======================================================
        private IExpectationModel expectationModel = new ExpectationModel();
        private IProcessStandardModel processStandardModel = new ProcessStandardModel();
        private ObservableCollection<Course> _Courses;
        private ObservableCollection<CourseMeta> _CoursesMeta = new ObservableCollection<CourseMeta>();
        private ObservableCollection<CoursePermission> _CoursePermissions = new ObservableCollection<CoursePermission>();
        private ObservableCollection<ProcessStandard> _ProcessStandards = new ObservableCollection<ProcessStandard>();
        private ObservableCollection<LbcExpectationIndex> _ExpectationIndexList = new ObservableCollection<LbcExpectationIndex>();
        private ObservableCollection<Information> _InformationMessages = new ObservableCollection<Information>();
        private string _CurCurriculumItem;
        private FTP _FtpClient = new FTP();
        private Information _CurrentFtpMessage;

        // =======================================================
        // Public Properties
        // =======================================================
        public ObservableCollection<Course> Courses
        {
            get
            {
                return _Courses;
            }
            set
            {
                if (value != null)
                {
                    _Courses = value;
                    NotifyPropertyChanged("Courses");
                }
            }
        }
        public ObservableCollection<CourseMeta> CoursesMeta
        {
            get
            {
                return _CoursesMeta;
            }
            set
            {
                if (value != null)
                {
                    _CoursesMeta = value;
                    NotifyPropertyChanged("CoursesMeta");
                }
            }
        }
        public ObservableCollection<LbcExpectationIndex> ExpectationIndexList
        {
            get
            {
                return _ExpectationIndexList;
            }
        }
        public ObservableCollection<CoursePermission> UserCoursePermissions
        {
            get
            {
                return _CoursePermissions;
            }
            set
            {
                if (value != null)
                {
                    _CoursePermissions = value;
                    NotifyPropertyChanged("CoursePermissions");
                }
            }
        }
        public FTP FtpClient
        {
            get
            {
                return _FtpClient;
            }
            set
            {
                _FtpClient = value;
            }
        }
        public ObservableCollection<ProcessStandard> ProcessStandards
        {
            get
            {
                return _ProcessStandards;
            }
            set
            {
                if (value != null)
                {
                    _ProcessStandards = value;
                    NotifyPropertyChanged("ProcessStandards");
                }
            }
        }
        public ObservableCollection<Information> InformationMessages
        {
            get
            {
                return _InformationMessages;
            }
            set
            {
                if (value != null)
                {
                    _InformationMessages = value;
                    NotifyPropertyChanged("InformationMessages");
                }
            }
        }
        public Information CurrentFtpMessage
        {
            get
            {
                return _CurrentFtpMessage;
            }
            set
            {
                _CurrentFtpMessage = value;
                NotifyPropertyChanged("CurrentFtpMessage");
                NotifyPropertyChanged("InformationMessages");
            }
        }
        public string CurCurriculumItem
        {
            get
            {
                return _CurCurriculumItem;
            }
            set
            {
                if (value != null)
                {
                    _CurCurriculumItem = value;
                    NotifyPropertyChanged("CurCurriculumItem");
                }
            }
        }


        // =======================================================
        // Methods
        // =======================================================

        public MainViewModel()
        {
            //Enable reading objects after their deleted
            // *** If an error occurs here, make sure that the LBCDBContext has a Scope property 
            //http://www.telerik.com/community/forums/orm/getting-started/it-is-not-allowed-to-read-or-to-write-an-instance-marked-for-deletion.aspx
            Helper.Session.DbContext.Scope.TransactionProperties.ReadAfterDelete = true;
        }

        /// <summary>
        /// Saves any pending changes back to the database
        /// </summary>
        public bool SaveChanges()
        {
            try
            {
                Session.DbContext.SaveChanges();

                return true;
            }
            catch
            {
                //An error occured
                return false;
            }
        }

        /// <summary>
        /// Loads the expectation index list from the AC_Management database
        /// </summary>
        /// <returns></returns>
        public bool LoadExpectationIndex()
        {
            try
            {
                _ExpectationIndexList = expectationModel.LoadExpectationIndex();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get the ExpectationIndex for the given expectation start code
        /// This is used to load a courses expectation index
        /// </summary>
        /// <param name="courseStartCode"></param>
        /// <returns></returns>
        public LbcExpectationIndex GetExpectationIndexForCourse(long? courseStartCode)
        {
            return (from expList in _ExpectationIndexList where expList.CodeStart == courseStartCode.ToString() select expList).First();
        }

        public ObservableCollection<LbcExpectationIndex> LoadExpectationGrade(string source)
        {
            try
            {
                return expectationModel.GetExpectationIndexGrade(ref _ExpectationIndexList, source);
            }
            catch
            {
                return null;
            }
        }

        public ObservableCollection<LbcExpectationIndex> LoadExpectationArea(string source, string grade)
        {
            try
            {
                return expectationModel.GetExpectationIndexArea(ref _ExpectationIndexList, source, grade);
            }
            catch
            {
                return null;
            }
        }

        public ObservableCollection<LbcExpectationIndex> LoadExpectationCourse(string source, string grade, string area)
        {
            try
            {
                return expectationModel.GetExpectationIndexCourse(ref _ExpectationIndexList, source, grade, area);
            }
            catch
            {
                return null;
            }
        }

        public ObservableCollection<LbcExpectationIndex> LoadExpectationVersion(string source, string grade, string area, string course)
        {
            try
            {
                return expectationModel.GetExpectationIndexVersion(ref _ExpectationIndexList, source, grade, area, course);
            }
            catch
            {
                return null;
            }
        }

        public ObservableCollection<ProcessStandard> LoadAllProcessStandards()
        {
            return processStandardModel.LoadAllProcessStandards();
        }

        /// <summary>
        /// Load the expectation lists for each course
        /// </summary>
        public bool LoadCurriculumExpectations()
        {
            return expectationModel.LoadCurriculumExpectations(Courses, CoursesMeta);
        }

        /// <summary>
        /// Update the specified course's alignment and load the expectations into the course's metadata
        /// </summary>
        /// <param name="newIndex"></param>
        /// <param name="crs"></param>
        /// <returns></returns>
        public bool UpdateCourseAlignment(LbcExpectationIndex newIndex, Course crs)
        {
            return expectationModel.UpdateCourseExpectations(newIndex, crs, ref _CoursesMeta); 
        }

        /// <summary>
        /// Create a new course and add it to the current user's course list
        /// </summary>
        /// <returns></returns>
        public bool CreateNewCourse(ref CourseViewModel crsVM)
        {
            try
            {
                //Create the new Course
                Course newCrs = crsVM.CreateCourse(ref _CoursesMeta);
                    
                if (newCrs == null)
                {
                    return false;
                }

                Courses.Add(newCrs);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteCourse(Course crs)
        {
            ICourseModel crsModel = new CourseModel();

            try
            {

                Courses.Remove(crs);
                crsModel.DeleteCourse(crs);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                crsModel = null;
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
