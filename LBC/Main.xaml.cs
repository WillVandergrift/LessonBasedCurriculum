using System.Windows.Input;
using acManagement;
using lbcLibrary;
using System.Windows;
using Telerik.Windows.Controls.RichTextBoxUI;
using Telerik.Windows.Controls.RichTextBoxUI.Dialogs;
using Telerik.Windows.Documents.Proofing;
using System;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.DragDrop;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls.Primitives;

namespace LBC
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main
    {
        private DispatcherTimer tmrStartup = new DispatcherTimer();

        private ViewModel.MainViewModel mainVM = new ViewModel.MainViewModel();
        private ViewModel.UserViewModel userVM = new ViewModel.UserViewModel();
        private ViewModel.CourseViewModel courseVM = new ViewModel.CourseViewModel();
        private ViewModel.UnitViewModel unitVM = new ViewModel.UnitViewModel();
        private ViewModel.LessonViewModel lessonVM = new ViewModel.LessonViewModel();

        private BackgroundWorker LoadDistrictSettings = new BackgroundWorker();
        private BackgroundWorker LoadUser = new BackgroundWorker();
        private BackgroundWorker LoadProcessStandards = new BackgroundWorker();
        private BackgroundWorker LoadExpectations = new BackgroundWorker();
        private BackgroundWorker LoadExpectationIndex = new BackgroundWorker();

        private DispatcherTimer tmrLoadLists = new DispatcherTimer();
        private DispatcherTimer tmrSelectNewUnit = new DispatcherTimer();
        private DispatcherTimer tmrSelectNewLesson = new DispatcherTimer();
        private DispatcherTimer tmrLoadExpectationIndex = new DispatcherTimer();

        //Curriculum Tree Variables
        Boolean loadingCourse = false;
        Boolean creatingNewUnit;
        Boolean creatingNewLesson1;
        Boolean creatingNewLesson2;
        RadTreeViewItem currentCourse = new RadTreeViewItem();
        RadTreeViewItem currentUnit = new RadTreeViewItem();
        RadTreeViewItem currentLesson = new RadTreeViewItem();

        public Main()
        {
            InitializeComponent();

            DataContext = mainVM;

            //Event Handlers
            treeCurriculum.AddHandler(RadDragAndDropManager.DropQueryEvent,
                new EventHandler<DragDropQueryEventArgs>(OnDropInsideTreeViewDropQuery), true);

            //Background Worker Events
            LoadExpectationIndex.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadExpectationIndex_RunWorkerCompleted);
            LoadExpectationIndex.DoWork += new DoWorkEventHandler(LoadExpectationIndex_DoWork);
            LoadDistrictSettings.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadDistrictSettings_RunWorkerCompleted);
            LoadDistrictSettings.DoWork += new DoWorkEventHandler(LoadDistrictSettings_DoWork);
            LoadUser.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadUser_RunWorkerCompleted);
            LoadUser.DoWork += new DoWorkEventHandler(LoadUser_DoWork);
            LoadProcessStandards.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadProcessStandards_RunWorkerCompleted);
            LoadProcessStandards.DoWork += new DoWorkEventHandler(LoadProcessStandards_DoWork);
            LoadExpectations.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadExpectations_RunWorkerCompleted);
            LoadExpectations.DoWork += new DoWorkEventHandler(LoadExpectations_DoWork);

            //FTP Events
            mainVM.FtpClient.DownloadComplete += new Helper.FTP.OnDownloadComplete(FtpClient_DownloadComplete);
            mainVM.FtpClient.ProgressUpdate += new Helper.FTP.OnProgressUpdate(FtpClient_ProgressUpdate);
            mainVM.FtpClient.UploadComplete += new Helper.FTP.OnUploadComplete(FtpClient_UploadComplete);

            //Tab Events
            tabsLesson.SelectionChanged += new RadSelectionChangedEventHandler(tabsLesson_SelectionChanged);
            tabsLsnAlignment.SelectionChanged += new RadSelectionChangedEventHandler(tabsLsnAlignment_SelectionChanged);

            //Timer events
            tmrLoadLists.Interval = new TimeSpan(0, 0, 5);
            tmrLoadLists.Tick += new EventHandler(tmrLoadLists_Tick);
            tmrSelectNewUnit.Interval = new TimeSpan(0, 0, 2);
            tmrSelectNewUnit.Tick += new EventHandler(tmrSelectNewUnit_Tick);
            tmrSelectNewLesson.Interval = new TimeSpan(0, 0, 2);
            tmrSelectNewLesson.Tick += new EventHandler(tmrSelectNewLesson_Tick);
            tmrLoadExpectationIndex.Interval = new TimeSpan(0, 0, 2);
            tmrLoadExpectationIndex.Tick += new EventHandler(tmrLoadExpectationIndex_Tick);

            //Curriculum tree Events
            treeCurriculum.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);

            //Load user settings
            txtLoginDistrictCode.Text = Properties.Settings.Default.DistrictCode;

            //Hide the Course, Unit and Lesson ribbon tabs
            rbnTabCourse.Visibility = System.Windows.Visibility.Collapsed;
            rbnTabUnit.Visibility = System.Windows.Visibility.Collapsed;
            rbnTabLesson.Visibility = System.Windows.Visibility.Collapsed;

            InitializeTextEditors();

            //Hide the curriculum tabs
            HideCurriculumTabs();
            //Hide the text editor ribbon tabs
            HideTextEditorRibbonTabs();
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnAddUser_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Display the curriculum tree on the left docking area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnMainCurriculum_Click(object sender, RoutedEventArgs e)
        {
            panesNavigation.SelectedItem = paneCurriculumTree;
        }

        /// <summary>
        /// Display the User management tab on the left docking area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnMainUsers_Click(object sender, RoutedEventArgs e)
        {
            panesNavigation.SelectedItem = paneUsers;
            if (userVM.LoadDistrictUserList())
            {
                //Failed to delete the course
                mainVM.InformationMessages.Insert(0, new Helper.Information(
                 "Login Success",
                 "Loaded District User List",
                 "The list of district users was loaded successfully."));

                treeUsers.ItemsSource = userVM.Users;
            }
            else
            {
                //Failed to delete the course
                mainVM.InformationMessages.Insert(0, new Helper.Information(
                 "Warning",
                 "Failed to Load District User List",
                 "An error occured while loading the district's user list. If the problem continues, please try restarting the program."));

            }
        }

        /// <summary>
        /// Delete the selected course from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnCourseDeleteCourse_Click(object sender, RoutedEventArgs e)
        {
            if (courseVM.CurCourse != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the course: " + courseVM.CurCourse.Name + "?", "Confirm Course Deletion", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (mainVM.DeleteCourse(courseVM.CurCourse) == true)
                    {
                        courseVM.CurCourse = null;

                        //Successfully Deleted the course
                        mainVM.InformationMessages.Insert(0, new Helper.Information(
                         "CourseDelete",
                         "Course Delete Successfully",
                         "The course was deleted successfully."));
                    }
                    else
                    {
                        //Failed to delete the course
                        mainVM.InformationMessages.Insert(0, new Helper.Information(
                         "Warning",
                         "Failed to Delete Course",
                         "An error occured while deleting the course " + courseVM.CurCourse.Name + ". If the problem continues, please try restarting the program."));
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a Course from the Curriculum Tree to delete.");
            }
        }

        void tmrLoadExpectationIndex_Tick(object sender, EventArgs e)
        {
            busyIndicator.BusyContent = "Loading Expectation Index...";
            tmrLoadExpectationIndex.Stop();

            //Load the Expectation Index list on a seperate thread
            LoadExpectationIndex.RunWorkerAsync();
        }

        /// <summary>
        /// Start loading the Expectation Index list from the database on a seperate thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadExpectationIndex_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = mainVM.LoadExpectationIndex();
        }

        /// <summary>
        /// Check to see if the Expectation Index loaded successfully
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadExpectationIndex_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result == true)
            {
                //Successfully loaded the expectation index from the AC_Management database
                mainVM.InformationMessages.Insert(0, new Helper.Information(
                 "Download",
                 "Expectation Index Loaded",
                 "The Expectation Index was successfully loaded from the Academic Creations Database."));
            }
            else
            {
                //Failed to load the expectation index from the AC_Management database
                mainVM.InformationMessages.Insert(0, new Helper.Information(
                 "Warning",
                 "Expectation Index Failed to Load",
                 "An error occured while loading the Expectation Index from the database. Please check your internet connection and restart the program."));
            }

            treeCurriculum.CollapseAll();

            busyIndicator.IsBusy = false;
        }

        public void btnMainNewCourse_Click(object sender, RoutedEventArgs e)
        {
            if (mainVM.CreateNewCourse(ref courseVM))
            {
                //Something went wrong while trying to start the download
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Course",
                    "New Course Created!",
                    "You can find your new course in the curriculum tree under the name 'New Course'."));
            }
            else
            {
                //Something went wrong while trying to start the download
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Warning",
                    "Failed to create new Course.",
                    "Please check your internet connection and try again. If the problem persists, try restarting the program."));
            }
        }

        public void btnMainViewReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reports.LessonReport lsnReport = new Reports.LessonReport();

                lsnReport.DataSource = unitVM.CurUnit.Lessons;

                reportView.Report = lsnReport;
                reportView.RefreshReport();

                tabsMain.SelectedItem = tabReport;
            }
            catch
            {
                //An error occured while creating the report
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Warning",
                     "Failed to Create Report",
                    "An error occured while generating the report. Please try again. If you still experience trouble, save your work and restart the program."));
            }
        }

        public void btnLessonDeleteExpectation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LessonExpectation selectedExpectation = (LessonExpectation)gridLsnExpectations.SelectedItem;

                if (selectedExpectation != null)
                {
                    if (lessonVM.DeleteExpectation(selectedExpectation))
                    {
                        //The expectation was deleted successfully
                        mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                            "DeleteExpectation",
                             "Expectation Deleted",
                            "The lesson expectation was successfully deleted."));

                        gridLsnExpectations.Rebind();
                    }
                    else
                    {
                        //The expectation failed to delete
                        mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                            "Warning",
                            "Failed to delete Lesson Expectation.",
                            "An error occured while deleting the lesson expectation. Please check your internet connection and try restarting the program."));
                    }
                }
                else
                {
                    MessageBox.Show("No expectation selected at the lesson level.");
                }
            }
            catch
            {

            }
        }

        void tmrSelectNewLesson_Tick(object sender, EventArgs e)
        {
            try
            {
            tmrSelectNewLesson.IsEnabled = false;

            treeCurriculum.CollapseAll();

            creatingNewLesson1 = true;
            currentCourse = treeCurriculum.ContainerFromItemRecursive(courseVM.CurCourse);
            currentCourse.IsExpanded = true;
            }
            catch
            {
                //Fail silently. This code is only used for selecting the newly created item
            }
        }

        void tmrSelectNewUnit_Tick(object sender, EventArgs e)
        {
            try
            {
            tmrSelectNewUnit.IsEnabled = false;

            treeCurriculum.CollapseAll();

            creatingNewUnit = true;
            currentCourse = treeCurriculum.ContainerFromItemRecursive(courseVM.CurCourse);
            currentCourse.IsExpanded = true;
            }
            catch
            {
                //Fail silently. This code is only used for selecting the newly created item
            }
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            try
            {
                if (treeCurriculum.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    if (creatingNewUnit == true)
                    {
                        creatingNewUnit = false;
                        unitVM.CurUnit = courseVM.CurCourse.Units[courseVM.CurCourse.Units.Count - 1];
                        currentUnit = currentCourse.ItemContainerGenerator.ContainerFromItem(unitVM.CurUnit) as RadTreeViewItem;
                        currentUnit.IsSelected = true;
                    }
                    else if (creatingNewLesson1 == true)
                    {
                        creatingNewLesson1 = false;
                        creatingNewLesson2 = true;
                        currentUnit = currentCourse.ItemContainerGenerator.ContainerFromItem(unitVM.CurUnit) as RadTreeViewItem;
                        currentUnit.IsExpanded = true;
                    }
                    else if (creatingNewLesson2 == true)
                    {
                        creatingNewLesson2 = false;
                        lessonVM.CurLesson = unitVM.CurUnit.Lessons[unitVM.CurUnit.Lessons.Count - 1];
                        currentLesson = currentUnit.ItemContainerGenerator.ContainerFromItem(lessonVM.CurLesson) as RadTreeViewItem;
                        currentLesson.IsSelected = true;
                    }
                }
            }
            catch
            {
                //Fail silently. This code is only used to select the newly created item.
            }
        }

        /// <summary>
        /// Lesson Expectations tab changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tabsLsnAlignment_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateLessonInformationPane();
        }

        /// <summary>
        /// Lesson Tabs selected item changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tabsLesson_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateLessonInformationPane();
        }

        /// <summary>
        /// Display the appropriate information pane based on what lesson tab is selected
        /// </summary>
        private void UpdateLessonInformationPane()
        {
            if (lessonVM.CurLesson != null)
            {
                HideLessonInformationPanes();

                if (tabsLesson.SelectedItem == tabLsnAlignment)
                {
                    //The Alignment tab is selected

                    //Check the lesson alignment tabs to see which is selected
                    if (tabsLsnAlignment.SelectedItem == tabLsnAlignmentExpectations)
                    {
                        //Display the Expectations information pane
                        paneExpectations.Visibility = System.Windows.Visibility.Visible;
                        panesInformation.SelectedItem = paneExpectations;
                    }
                    else if (tabsLsnAlignment.SelectedItem == tabLsnAlignmentPS)
                    {
                        //Display the Process Standards information pane
                        paneProcessStandards.Visibility = System.Windows.Visibility.Visible;
                        panesInformation.SelectedItem = paneProcessStandards;
                    }
                }
                else
                {
                    //By default, display the infomration pane
                    panesInformation.SelectedItem = paneInformation;
                }
            }
        }

        private void HideLessonInformationPanes()
        {
            paneExpectations.Visibility = System.Windows.Visibility.Collapsed;
            paneProcessStandards.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// The upload operation completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FtpClient_UploadComplete(object sender, Atp.AsyncMethodCompletedEventArgs e)
        {
            Document uploadedDoc = (Document)mainVM.CurrentFtpMessage.MetaData;

            //Move the message to the top of the list
            mainVM.InformationMessages.Move(mainVM.InformationMessages.IndexOf(mainVM.CurrentFtpMessage), 0);
            mainVM.CurrentFtpMessage.Header = String.Format("Upload Complete - {0}", uploadedDoc.Filename);
            mainVM.CurrentFtpMessage.Message = uploadedDoc.Filename + " was uploaded successfully.";
            mainVM.CurrentFtpMessage.UpdateIcon("UploadComplete");
            lstInformation.Items.Refresh();

            gridLsnDocuments.Rebind();
            mainVM.SaveChanges();
        }

        /// <summary>
        /// Delete the selected document and remove it from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnLessonDeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            //Get the selected lesson document
            string filePath;
            string filename;
            Document selectedDoc = (Document)gridLsnDocuments.SelectedItem;

            try
            {
                if (selectedDoc != null)
                {
                    filePath = "/" + Helper.Session.CurDistrict.DistrictCode.Substring(1) + "/" + lessonVM.CurLesson.ID + "/" + selectedDoc.Filename;
                    filename = selectedDoc.Filename;

                    if (mainVM.FtpClient.DeleteFile(filePath))
                    {
                        //Delete the record from the database
                        lessonVM.DeleteDocument(selectedDoc);

                        //The document was deleted successfully
                        mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                            "DeleteDocument",
                             filename + " Deleted",
                            "The document was successfully deleted."));

                        gridLsnDocuments.Rebind();
                        mainVM.SaveChanges();
                    }
                    else
                    {
                        //Something went wrong while trying to delete the document
                        mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                            "Warning",
                            "Failed to Delete Document.",
                            "Please check your internet connection and try again. If the problem persists, try restarting the program."));
                    }
                }
                else
                {
                    //No document is selected
                    MessageBox.Show("Please select a document to delete from the Documents tab located in the lesson's content tab.","No Document Selected");
                }
            }
            catch
            {
                //Something went wrong while trying to delete the document
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Warning",
                    "Upload failed to start.",
                    "Please check your internet connection and try again. If the problem persists, try restarting the program."));
            }
        }

        /// <summary>
        /// Add a new lesson document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnLessonNewDocument_Click(object sender, RoutedEventArgs e)
        {
            string source;
            string sourceFile;
            string destination;
            Document newDoc;

            try
            {
                //Make sure we're not already downloading a file
                if (mainVM.FtpClient.IsBusy)
                {
                    MessageBox.Show("Another FTP operation is already in progress. Please wait for it to finish before trying to upload another file.", "FTP operation in progress");
                    return;
                }

                //Get the source file from the user
                System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();

                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                source = dlg.FileName;
                sourceFile = source.Substring(source.LastIndexOf("\\") + 1);
                //Build the destination path
                destination = "/" + Helper.Session.CurDistrict.DistrictCode.Substring(1) + "/" + lessonVM.CurLesson.ID + "/" + sourceFile;

                //Add the document information to the database
                newDoc = lessonVM.AddDocument(sourceFile, destination);

                //Make sure the database record was created successfully before uploading the actual document
                if (newDoc == null)
                {
                    //Something went wrong while trying to start the upload
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "Warning",
                        "Upload failed to start.",
                        "Please check your internet connection and try again. If the problem persists, try restarting the program."));

                    return;
                }

                //Start uploading the document to the server
                mainVM.FtpClient.BeginUpload(source, destination);

                //The upload started successfully
                mainVM.CurrentFtpMessage = new LBC.Helper.Information(
                    "UploadDocument",
                    "Uploading " + sourceFile,
                    "0% Complete",
                    "Upload",
                    newDoc,
                    string.Empty);

                mainVM.InformationMessages.Insert(0, mainVM.CurrentFtpMessage);
            }
            catch
            {
                //Something went wrong while trying to start the upload
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Warning",
                    "Upload failed to start.",
                    "Please check your internet connection and try again. If the problem persists, try restarting the program."));
            }
        }

        /// <summary>
        /// The progress of our ftp download has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FtpClient_ProgressUpdate(object sender, Atp.IO.FileSystemProgressEventArgs e)
        {
            mainVM.CurrentFtpMessage.Message = Math.Round(e.Percentage,1) + "% Complete";
            lstInformation.Items.Refresh();
        }

        /// <summary>
        /// Our FTP download completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FtpClient_DownloadComplete(object sender, Atp.AsyncMethodCompletedEventArgs e)
        {
            Document downloadedDoc = (Document)mainVM.CurrentFtpMessage.MetaData;

            //Move the message to the top of the list
            mainVM.InformationMessages.Move(mainVM.InformationMessages.IndexOf(mainVM.CurrentFtpMessage), 0);
            mainVM.CurrentFtpMessage.Header = String.Format("Download Complete - {0}", downloadedDoc.Filename);
            mainVM.CurrentFtpMessage.Message = "Saved to: " + mainVM.CurrentFtpMessage.Tag;
            mainVM.CurrentFtpMessage.UpdateIcon("DownloadComplete");
            lstInformation.Items.Refresh();
        }

        /// <summary>
        /// Load the user's settings from the database using a separate thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadUser_DoWork(object sender, DoWorkEventArgs e)
        {
            List<String> credentials;

            credentials = (List<String>)e.Argument;

            e.Result = userVM.AuthenticateUser(credentials[0], credentials[1]);
        }

        /// <summary>
        /// The load user operation has completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadUser_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LbcUser authUser = (LbcUser)e.Result;

            if (authUser != null)
            {
                Helper.Session.CurUser = authUser;
                txtLoginPassword.Password = string.Empty;
                txtLoginUser.Text = string.Empty;

                mainVM.InformationMessages.Insert(0, LBC.Helper.Information.UserGreeting(Helper.Session.CurUser));
                userVM.UpdateUserLastLogin();

                mainVM.Courses = userVM.LoadCurriculum(Helper.Session.CurUser, mainVM.UserCoursePermissions, mainVM.CoursesMeta);
                if (mainVM.Courses == null)
                {
                    //Inform the user that an error occured while loading the courses
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "Warning",
                        "Curriculum failed to load",
                        "An error occured while loading the user's curriculum. Please try reloading the page and logging in again."));
                }
                else if (mainVM.Courses.Count == 0)
                {
                    //Inform the user that no Courses were loaded
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "Info",
                        "No Courses were found",
                        "No courses were found for " + Helper.Session.CurUser.FirstName + " " + Helper.Session.CurUser.LastName + "."));
                }

                //Populate our curriculum tree
                treeCurriculum.ItemsSource = mainVM.Courses;
                treeCurriculum.ExpandAll();

                //Start the timer for loading lists
                tmrLoadLists.Start();
            }
            else
            {
                busyIndicator.IsBusy = false;

                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Warning",
                    "Login Failed",
                    "Failed to login with the given credentials. Please try again. If this error continues, please check your internet connection."));
                txtLoginPassword.Password = string.Empty;
                txtLoginUser.Text = string.Empty;
                txtLoginUser.Focus();
            }
        }

        /// <summary>
        /// Load the district's settings from the database using a seperate thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadDistrictSettings_DoWork(object sender, DoWorkEventArgs e)
        {

            e.Result = Helper.Session.LoadDistrictSettings((string)e.Argument);
        }

        /// <summary>
        /// The District Settings load operation has completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadDistrictSettings_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result == true)
            {
                //Display the distric settings loaded message in the information panel
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Settings",
                    Helper.Session.CurDistrict.DistrictName + " School District",
                    "District settings were found for the " + Helper.Session.CurDistrict.DistrictName + " School District."));

                //Update the busy indicator
                busyIndicator.BusyContent = "Loading Curriculum...";

                //Load user settings
                List<String> loginCredentials = new List<string>();
                loginCredentials.Add(txtLoginUser.Text);
                loginCredentials.Add(txtLoginPassword.Password);
                LoadUser.RunWorkerAsync(loginCredentials);
            }
            else
            {
                //Inform the user that an error occured while retrieving district settings
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Warning",
                    "Failed to retrieve district settings",
                    "There was an error loading district settings for the given district code."));

                busyIndicator.IsBusy = false;
                popupLogin.Show();
            }
        }

        /// <summary>
        /// Start loading our Process Standards
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmrLoadLists_Tick(object sender, EventArgs e)
        {
            busyIndicator.BusyContent = "Loading Process Standards...";
            tmrLoadLists.Stop();
            LoadProcessStandards.RunWorkerAsync();
        }

        /// <summary>
        /// Start loading the Expectations in a separate thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadExpectations_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = mainVM.LoadCurriculumExpectations();
        }

        /// <summary>
        /// Expectations background worker has completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadExpectations_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result == true)
            {
                mainVM.InformationMessages.Insert(0, new Helper.Information(
                    "Download",
                    "Course Expectations",
                    "The list of Expectations has been successfully loaded into the program."));

                treeCurriculum.Items.Refresh();
            }
            else
            {
                mainVM.InformationMessages.Insert(0, new Helper.Information(
                        "Warning",
                        "Expectations failed to load",
                        "The list of Expectations failed to load."));
            }

            //Load the Expectation Index List
            tmrLoadExpectationIndex.Start();

        }

        /// <summary>
        /// Start loading the Process Standards in a separate thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadProcessStandards_DoWork(object sender, DoWorkEventArgs e)
        {
            //Load all the Process Standards into the program
            e.Result = mainVM.LoadAllProcessStandards();
        }

        /// <summary>
        /// Process Standards background worker has completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadProcessStandards_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mainVM.ProcessStandards = (ObservableCollection<ProcessStandard>)e.Result;

            if (mainVM.ProcessStandards != null)
            {
                if (mainVM.ProcessStandards.Count > 0)
                {
                    mainVM.InformationMessages.Insert(0, new Helper.Information(
                        "Download",
                        "Process Standards Loaded",
                        "The list of Process Standards has been successfully loaded into the program."));
                }
                else
                {
                    mainVM.InformationMessages.Insert(0, new Helper.Information(
                         "Warning",
                         "Process Standards failed to load",
                         "The list of Process Standards failed to load."));
                }
            }
            else
            {
                mainVM.InformationMessages.Insert(0, new Helper.Information(
                     "Warning",
                     "Process Standards failed to load",
                     "The list of Process Standards failed to load."));
            }

            //Set the Process Strandard grid's datacontext
            gridProcessStandards.DataContext = mainVM;

            busyIndicator.BusyContent = "Loading Expectation Lists...";

            //Load expectation lists for all courses
            LoadExpectations.RunWorkerAsync();
        }

        private void InitializeTextEditors()
        {
            rtbLsnContentDescription.FindReplaceDialog = new FindReplaceDialog();
            rtbLsnContentDescription.ParagraphPropertiesDialog = new RadParagraphPropertiesDialog();
            rtbLsnContentDescription.FontPropertiesDialog = new FontPropertiesDialog();

            rtbLsnContentDescription.InsertSymbolWindow = new RadInsertSymbolDialog();
            rtbLsnContentDescription.InsertHyperlinkDialog = new RadInsertHyperlinkDialog();

            rtbLsnContentDescription.ContextMenu = new Telerik.Windows.Controls.RichTextBoxUI.ContextMenu();
            rtbLsnContentDescription.SelectionMiniToolBar = new SelectionMiniToolBar();

            rtbLsnContentDescription.InsertTableDialog = new InsertTableDialog();
            rtbLsnContentDescription.TablePropertiesDialog = new TablePropertiesDialog();
            rtbLsnContentDescription.TableBordersDialog = new TableBordersDialog();

            rtbLsnContentDescription.SpellCheckingDialog = new SpellCheckingDialog();
            //((DocumentSpellChecker)rtbLsnContentDescription.SpellChecker).AddDictionary(new RadEn_USDictionary(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Display the login popup window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMainLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                popupLogin.Close();
                popupLogin.ShowDialog();
                txtLoginUser.Focus();
            }
            catch
            {

            }

        }

        /// <summary>
        /// Create a new Unit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnCourseNewUnit_Click(object sender, RoutedEventArgs e)
        {
            if (unitVM.CreateNewUnit(courseVM.CurCourse) == true)
            {
                //Inform the user that the Unit was created successfully
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Unit",
                    "Unit Created Successfully",
                    "The unit was created successfully."));
                
                //Refresh the curriculum tree and then expand the current course
                treeCurriculum.Items.Refresh();
                tmrSelectNewUnit.IsEnabled = true;
            }
            else
            {
                //Inform the user that an error occured while creating the unit
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Warning",
                    "Failed to create unit",
                    "There was an error creating the unit."));
            }
        }

        /// <summary>
        /// Delete the current unit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnUnitDeleteUnit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the unit: " + unitVM.CurUnit.UnitName + "?", "Confirm Unit Delete", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                if (unitVM.DeleteUnit(unitVM.CurUnit) == true)
                {
                    //Inform the user that the unit was deleted successfully
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "UnitDelete",
                        "Unit Deleted",
                        "The unit was deleted successfully."));

                    //Refresh the curriculum tree and then expand the current course
                    treeCurriculum.Items.Refresh();

                    //Hide the unit ribbon tab
                    rbnTabUnit.Visibility = System.Windows.Visibility.Collapsed;
                    ribbonMain.SelectedTab = rbnTabMain;
                }
                else
                {
                    //Inform the user that an error occured while deleting the unit
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "Warning",
                        "Failed to delete unit",
                        "There was an error deleting the unit."));
                }
            }
        }

        /// <summary>
        /// Create a new Lesson
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnitNewLesson_Click(object sender, RoutedEventArgs e)
        {
            if (lessonVM.CreateNewLesson(unitVM.CurUnit) == true)
            {
                //Inform the user that the Unit was created successfully
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Lesson",
                    "Lesson Created Successfully",
                    "The lesson was created successfully."));

                //Refresh the curriculum tree and then expand the current course
                treeCurriculum.Items.Refresh();
                tmrSelectNewLesson.IsEnabled = true;
            }
            else
            {
                //Inform the user that an error occured while creating the unit
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Warning",
                    "Failed to create lesson",
                    "There was an error creating the lesson."));
            }
        }

        /// <summary>
        /// Delete the current lesson
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnLessonDeleteLesson_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the lesson: " + lessonVM.CurLesson.LessonName + "?", "Confirm Lesson Delete", MessageBoxButton.YesNoCancel) ==  MessageBoxResult.Yes) 
            {
                if (lessonVM.DeleteLesson(lessonVM.CurLesson) == true)
                {
                    //Inform the user that the lesson was deleted successfully
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "LessonDelete",
                        "Lesson Deleted",
                        "The lesson was deleted successfully."));

                    //Refresh the curriculum tree and then expand the current course
                    treeCurriculum.Items.Refresh();

                    //Hide the lesson ribbon tab
                    rbnTabLesson.Visibility = System.Windows.Visibility.Collapsed;
                    ribbonMain.SelectedTab = rbnTabMain;
                }
                else
                {
                    //Inform the user that an error occured while deleting the lesson
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "Warning",
                        "Failed to delete lesson",
                        "There was an error deleting the lesson."));
                }
            }
        }
        
        /// <summary>
        /// Attempt to authenticate the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoginOK_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Login()
        {
            //Validate the login dialog
            if (validateLoginDialog() == false)
            {
                //Inform the user that there were errors with the login dialog
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Warning",
                    "Login Error",
                    "Pleaes correct the fields marked in red, and try logging in again."));

                return;
            }

            //Save User District Code
            Properties.Settings.Default.DistrictCode = txtLoginDistrictCode.Text;
            Properties.Settings.Default.Save();

            popupLogin.Close();

            //Display the busy indicator
            busyIndicator.BusyContent = "Loading District Settings...";
            busyIndicator.IsBusy = true;

            //Try to find the district settings in the database
            LoadDistrictSettings.RunWorkerAsync("-" + txtLoginDistrictCode.Text);
        }

        /// <summary>
        /// Make sure that the user filled out the login dialog correctly
        /// </summary>
        /// <returns></returns>
        private bool validateLoginDialog()
        {
            bool valid = true;
            Double districtCode;

            //Reset our login field visuals
            SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);
            SolidColorBrush errorBrush = new SolidColorBrush(Colors.Red);

            txtLoginDistrictCode.Background = whiteBrush;
            txtLoginUser.Background = whiteBrush;
            txtLoginPassword.Background = whiteBrush;

            //District Code
            if (txtLoginDistrictCode.Text.Length == 6)
            {
                if (Double.TryParse(txtLoginDistrictCode.Text, out districtCode) == false)
                {
                    valid = false;
                    txtLoginDistrictCode.Background = errorBrush;
                }
            }
            else
            {
                valid = false;
                txtLoginDistrictCode.Background = errorBrush;
            }


            //User name
            if (txtLoginUser.Text.Length == 0)
            {
                valid = false;
                txtLoginUser.Background = errorBrush;
            }

            //Password
            if (txtLoginPassword.Password.Length == 0)
            {
                valid = false;
                txtLoginPassword.Background = errorBrush;
            }

            return valid;
        }

        /// <summary>
        /// Close the login window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoginCancel_Click(object sender, RoutedEventArgs e)
        {
            popupLogin.Close();
        }

        /// <summary>
        /// Set the editor that has focus to the current editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RichTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            RadRichTextBox curEditor = (RadRichTextBox)sender;

            ribbonMain.DataContext = curEditor.Commands;

            ShowTextEditorRibbonTabs();
        }

        /// <summary>
        /// Display the curriculum Item's properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeCurriculum_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            HideCurriculumTabs();

            if (treeCurriculum.SelectedItem == null)
                return;

            switch (treeCurriculum.SelectedItem.GetType().ToString())
            {
                case "lbcLibrary.Course":
                    loadingCourse = true;
                    mainVM.CurCurriculumItem = "Course";
                    courseVM.CurCourse = (Course)treeCurriculum.SelectedItem;
                    courseVM.UpdateCourseMeta(mainVM.CoursesMeta);
                    //Update list bindings
                    UpdateListBindings();
                    tabsMain.SelectedItem = tabCourse;
                    rbnTabCourse.Visibility = System.Windows.Visibility.Visible;
                    rbnTabUnit.Visibility = System.Windows.Visibility.Collapsed;
                    rbnTabLesson.Visibility = System.Windows.Visibility.Collapsed;
                    ribbonMain.SelectedTab = rbnTabCourse;
                    //Update course bindings
                    UpdateCourseBinding();
                    //Update the Alignment Tabs
                    UpdateCourseAlignment();
                    //Display the Course Tab
                    tabsCourse.Visibility = Visibility.Visible;
                    //Hide Unrelated Information Panes
                    HideLessonInformationPanes();
                    loadingCourse = false;
                    break;
                case "lbcLibrary.Unit":
                    mainVM.CurCurriculumItem = "Unit";
                    unitVM.CurUnit = (Unit)treeCurriculum.SelectedItem;
                    courseVM.CurCourse = unitVM.CurUnit.Course;
                    courseVM.UpdateCourseMeta(mainVM.CoursesMeta);
                    //Update list bindings
                    UpdateListBindings();
                    tabsMain.SelectedItem = tabUnit;
                    rbnTabCourse.Visibility = System.Windows.Visibility.Collapsed;
                    rbnTabUnit.Visibility = System.Windows.Visibility.Visible;
                    rbnTabLesson.Visibility = System.Windows.Visibility.Collapsed;
                    ribbonMain.SelectedTab = rbnTabUnit;
                    //Update unit bindings
                    UpdateUnitBinding();
                    //Display the Unit tab
                    tabsUnit.Visibility = Visibility.Visible;
                    //Hide Unrelated Information Panes
                    HideLessonInformationPanes();
                    break;
                case "lbcLibrary.Lesson":
                    mainVM.CurCurriculumItem = "Lesson";
                    lessonVM.CurLesson = (Lesson)treeCurriculum.SelectedItem;
                    unitVM.CurUnit = lessonVM.CurLesson.Unit;
                    courseVM.CurCourse = unitVM.CurUnit.Course;
                    courseVM.UpdateCourseMeta(mainVM.CoursesMeta);
                    //Update list bindings
                    UpdateListBindings();
                    tabsMain.SelectedItem = tabLesson;
                    rbnTabCourse.Visibility = System.Windows.Visibility.Collapsed;
                    rbnTabUnit.Visibility = System.Windows.Visibility.Collapsed;
                    rbnTabLesson.Visibility = System.Windows.Visibility.Visible;
                    ribbonMain.SelectedTab = rbnTabLesson;
                    //Update lesson bindings
                    UpdateLessonBinding();
                    //Display the lesson tab
                    tabsLesson.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// Update the bindings for controls in the course tab
        /// </summary>
        private void UpdateCourseBinding()
        {
            tabsCourse.DataContext = null;
            tabsCourse.DataContext = courseVM.CurCourse;
        }

        /// <summary>
        /// Update the bindings for controls in the unit tab
        /// </summary>
        private void UpdateUnitBinding()
        {
            tabsUnit.DataContext = null;
            tabsUnit.DataContext = unitVM.CurUnit;
        }

        /// <summary>
        /// Update the bindings for controls in the lesson tab
        /// </summary>
        private void UpdateLessonBinding()
        {
            tabsLesson.DataContext = null;
            tabsLesson.DataContext = lessonVM.CurLesson;
            tabsLsnWorkspace.DataContext = null;
            tabsLsnWorkspace.DataContext = lessonVM;
        }

        /// <summary>
        /// Updates the bindings for the alignment combo boxes
        /// </summary>
        private void UpdateCourseAlignment()
        {
            UpdateCourseAlignmentSource();

            if (courseVM.CurCourse.ExpectationCodeStart != null)
            {
                //Display the alignment information for the course
                DisplayAlignmentInfo(mainVM.GetExpectationIndexForCourse(courseVM.CurCourse.ExpectationCodeStart));
            }
        }

        private void DisplayAlignmentInfo(LbcExpectationIndex alignment)
        {
            cboAlign1Source.Text = alignment.Txt_source;
            cboAlign1Grade.Text = alignment.Txt_grade;
            cboAlign1Area.Text = alignment.Txt_area;
            cboAlign1Course.Text = alignment.Txt_course;
            cboAlign1Version.Text = alignment.Version.ToString();
        }

        /// <summary>
        /// Update the Source Combo Box for the course alignment
        /// </summary>
        private void UpdateCourseAlignmentSource()
        {
            //Source Combo Box
            cboAlign1Source.Items.Clear();

            foreach (LbcExpectationIndex ex in mainVM.ExpectationIndexList)
            {
                bool unique = true;

                //Only add unique items
                foreach (LbcExpectationIndex item in cboAlign1Source.Items)
                {
                    if (item.Txt_source == ex.Txt_source)
                    {
                        unique = false;
                    }
                }

                if (unique == true)
                {
                    cboAlign1Source.Items.Add(ex);
                }
            }    
        }

        /// <summary>
        /// Update the bindings for lists found in the information pane
        /// </summary>
        private void UpdateListBindings()
        {
            gridExpectations.DataContext = courseVM.CurCourseMeta;
        }

        private void gridProcessStandards_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProcessStandard selectedStandard = (ProcessStandard)gridProcessStandards.SelectedItem;

            lessonVM.CurLesson.ProcessStandards.Add(selectedStandard);
            gridLsnProcessStandards.Rebind();
       }

        private void gridExpectations_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Expectation selectedExpectation = (Expectation)gridExpectations.SelectedItem;

            switch (mainVM.CurCurriculumItem)
            {
                case "Lesson":
                    LessonExpectation lsnExpectation = lessonVM.AddLessonExpectation(selectedExpectation);

                    if (lsnExpectation != null)
                    {
                        lessonVM.CurLesson.LessonExpectations.Add(lsnExpectation);
                        gridLsnExpectations.Rebind();
                    }
                    break;
            }
        } 

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        private void SaveChanges()
        {
            if (mainVM.Courses != null)
            {
                if (mainVM.Courses.Count == 0)
                {
                    return;
                }

                if (mainVM.SaveChanges() == true)
                {
                    mainVM.InformationMessages.Insert(0, new Helper.Information(
                     "Save",
                     "Save Complete (" + DateTime.Now.ToShortTimeString() + ")",
                     "Your changes have been successfully saved."));
                }
                else
                {
                    mainVM.InformationMessages.Insert(0, new Helper.Information(
                     "Warning",
                     "Save Failed",
                     "An error occured while saving changes."));
                }
            }
        }

        /// <summary>
        /// Handle Drag/Drop events for the curriculum tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeCurriculum_DragEnded(object sender, RadTreeViewDragEndedEventArgs e)
        {
            //Dragging a unit into a course
            if (e.DraggedItems[0].GetType().ToString() == "lbcLibrary.Unit")
            {
                if (e.TargetDropItem.FullPath == "lbcLibrary.Course")
                {
                    Course tmpCourse = (Course)e.TargetDropItem.Item;
                    Unit tmpUnit = (Unit)e.DraggedItems[0];

                    if (unitVM.MoveUnit(tmpUnit, tmpCourse) == true)
                    {
                        mainVM.InformationMessages.Insert(0, new Helper.Information(
                         "MoveUnit",
                         "Unit Moved Successfully",
                         "The unit: " + tmpUnit.UnitName + " was successfully moved into " + tmpCourse.Name));

                        //Refresh the curriculum tree
                        treeCurriculum.Items.Refresh();
                    }
                    else
                    {
                        mainVM.InformationMessages.Insert(0, new Helper.Information(
                         "Warning",
                         "Unit Move Failed",
                         "An error occured while moving the unit."));
                    }
                }
            }
            //Dragging a lesson into a unit
            else if (e.DraggedItems[0].GetType().ToString() == "lbcLibrary.Lesson")
            {
                if (e.TargetDropItem.FullPath == "lbcLibrary.Course\\lbcLibrary.Unit")
                {
                    Unit tmpUnit = (Unit)e.TargetDropItem.Item;
                    Lesson tmpLesson = (Lesson)e.DraggedItems[0];

                    if (lessonVM.MoveLesson(tmpLesson, tmpUnit) == true)
                    {
                        mainVM.InformationMessages.Insert(0, new Helper.Information(
                         "MoveLesson",
                         "Lesson Moved Successfully",
                         "The lesson: " + tmpLesson.LessonName + " was successfully moved into " + tmpUnit.UnitName));

                        //Refresh the curriculum tree
                        treeCurriculum.Items.Refresh();
                    }
                    else
                    {
                        mainVM.InformationMessages.Insert(0, new Helper.Information(
                         "Warning",
                         "Lesson Move Failed",
                         "An error occured while moving the lesson."));
                    }
                }
            }
        }

        /// <summary>
        /// Validate Drag/Drop events for the curriculum tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDropInsideTreeViewDropQuery(object sender, DragDropQueryEventArgs e)
        {
            try
            {
                var treeViewItem = e.Options.Destination as RadTreeViewItem;
                if (treeViewItem.DropPosition != DropPosition.Inside)
                {
                    e.QueryResult = false;
                }
                else
                {
                    //Don't allow courses to be dropped into other curriculum items
                    if (e.OriginalSource.ToString() == "lbcLibrary.Course")
                    {
                        e.QueryResult = false;
                    }
                    //Only allow units to be dropped into courses
                    else if (e.OriginalSource.ToString() == "lbcLibrary.Unit" && e.Options.Destination.ToString() != "lbcLibrary.Course")
                    {
                        e.QueryResult = false;
                    }
                    //Only allow lessons to be dropped into units
                    else if (e.OriginalSource.ToString() == "lbcLibrary.Lesson" && e.Options.Destination.ToString() != "lbcLibrary.Unit")
                    {
                        e.QueryResult = false;
                    }
                }
            }
            catch
            {
                //Fail Silently
            }
        }

        /// <summary>
        /// Hide the ribbon text editor tabs unless the ribbon is now focused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Helper.GUI.RibbonHasFocus((DependencyObject)Keyboard.FocusedElement) == false)
            {
                HideTextEditorRibbonTabs();
            }
        }

        /// <summary>
        /// Show the text editor ribbon tabs
        /// </summary>
        private void ShowTextEditorRibbonTabs()
        {
            rbnTabHome.Visibility = Visibility.Visible;
            rbnTabInsert.Visibility = Visibility.Visible;
            rbnTabReview.Visibility = Visibility.Visible;
            rbnTabTable.Visibility = Visibility.Visible;

            ribbonMain.SelectedTab = rbnTabHome;
        }

        /// <summary>
        /// Hide the text editor ribbon tabs
        /// </summary>
        private void HideTextEditorRibbonTabs()
        {
            rbnTabHome.Visibility = Visibility.Hidden;
            rbnTabInsert.Visibility = Visibility.Hidden;
            rbnTabReview.Visibility = Visibility.Hidden;
            rbnTabTable.Visibility = Visibility.Hidden;

            ribbonMain.SelectedTab = rbnTabMain;
        }

        /// <summary>
        /// Hide the main curriculum tabs
        /// </summary>
        private void HideCurriculumTabs()
        {
            tabsCourse.Visibility = Visibility.Hidden;
            tabsUnit.Visibility = Visibility.Hidden;
            tabsLesson.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Open the document or URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridLsnDocuments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Document selectedDocument = (Document)gridLsnDocuments.SelectedItem;

            if (selectedDocument.FilePath == "URL")
            {
                //If the item is a url, open the webpage
                try
                {
                    //Open the web browser 
                    Process.Start(selectedDocument.Filename);
                }
                catch
                {
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "Warning",
                        "Web Page Failed to Open",
                        "The web page failed to open. Please check to make sure the address is valid"));

                }
            }
            else
            {
                //If the item is a document
                try
                {
                    //Make sure we're not already downloading a file
                    if (mainVM.FtpClient.IsBusy)
                    {
                        MessageBox.Show("Another FTP is already in progress. Please wait for it to finish before trying to download another file.", "FTP operation in progress");
                        return;
                    }

                    //Get the destination file from the user
                    System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                    dlg.FileName = selectedDocument.Filename;
                    dlg.Filter = Helper.FTP.BuildDialogExtension(selectedDocument.Filename);


                    if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }

                    string destination = dlg.FileName;

                    //Build the remote file path
                    string source = String.Format("/{0}/{1}/{2}", Helper.Session.CurDistrict.DistrictCode.Substring(1), lessonVM.CurLesson.ID, selectedDocument.Filename);

                    if (mainVM.FtpClient.BeginDownload(source, destination))
                    {
                        //The download started successfully
                        mainVM.CurrentFtpMessage = new LBC.Helper.Information(
                            "DownloadDocument",
                            "Downloading " + selectedDocument.Filename,
                            "0% Complete",
                            "Download",
                            selectedDocument,
                            destination);

                        mainVM.InformationMessages.Insert(0, mainVM.CurrentFtpMessage);
                    }
                    else
                    {
                        //Something went wrong while trying to start the download
                        mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                            "Warning",
                            "Download failed to start.",
                            "Please check your internet connection and try again. If the problem persists, try restarting the program."));
                    }
                }
                catch
                {
                    //Something went wrong while trying to start the download
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "Warning",
                        "Download failed to start.",
                        "Please check your internet connection and try again. If the problem persists, try restarting the program."));
                }
            }
        }

        /// <summary>
        /// Check to see what type of information message was clicked, and perform the appropriate action on it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstInformation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Helper.Information selectedInformation = (Helper.Information)lstInformation.SelectedItem;
            
                //No item was selected
                if (selectedInformation == null)
                {
                    return;
                }

                switch (selectedInformation.InformationType)
                {
                    case "Download":
                        Process.Start(selectedInformation.Tag);
                        break;
                }
            }
            catch
            {
                //Running the command associated with the information item failed
            }
        }

        /// <summary>
        /// Allow the user to login by pressing the enter key when the password field is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLoginPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Login();
            }
        }

        /// <summary>
        /// Filter the curriculum tree based on the value entered in txtCurriculumFilter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCurriculumFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchValue = txtCurriculumFilter.Text;

            //Show / Hide the filter label that sits on top of txtCurriculumFilter
            if (searchValue.Length > 0)
            {
                lblCourseFilter.Visibility = System.Windows.Visibility.Hidden;
                paneCurriculumTree.Header = "Curriculum Tree (Filtered)";
            }
            else
            {
                lblCourseFilter.Visibility = System.Windows.Visibility.Visible;
                paneCurriculumTree.Header = "Curriculum Tree";
            }

            foreach (Object curItem in treeCurriculum.Items)
            {
                switch (curItem.GetType().ToString().ToLower())
                {
                    case "lbclibrary.course":
                        Course tmpCourse = (Course)curItem;
                        RadTreeViewItem item = treeCurriculum.GetItemByPath(tmpCourse.Name);

                        if (tmpCourse.Name.Contains(searchValue))
                        {                     
                            item.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            item.Visibility = System.Windows.Visibility.Hidden;
                            item.Visibility = System.Windows.Visibility.Collapsed;
                        }

                        break;
                    case "lbclibrary.unit":
                        break;
                    case "lbclibrary.lesson":
                        break;
                    default:
                        break;
                }
            }
        }

        private void btnAddCourse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mainVM.CreateNewCourse(ref courseVM))
            {
                //Something went wrong while trying to start the download
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Course",
                    "New Course Created!",
                    "You can find your new course in the curriculum tree under the name 'New Course'."));
            }
            else
            {
                //Something went wrong while trying to start the download
                mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                    "Warning",
                    "Failed to create new Course.",
                    "Please check your internet connection and try again. If the problem persists, try restarting the program."));
            }
        }

        /// <summary>
        /// Update the grade alignment combo boxes binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboAlign1Source_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			cboAlign1Grade.Items.Clear();
			cboAlign1Area.Items.Clear();
			cboAlign1Course.Items.Clear();
			cboAlign1Version.Items.Clear();
			
            if (cboAlign1Source.SelectedItem != null)
            {
                UpdateCourseAlignmentGrade();
            }
        }

        /// <summary>
        /// Update the Grade Combo Box for the course alignment
        /// </summary>
        private void UpdateCourseAlignmentGrade()
        {
            ObservableCollection<LbcExpectationIndex> tmpIndex = new ObservableCollection<LbcExpectationIndex>();
            LbcExpectationIndex selectedItem = (LbcExpectationIndex)cboAlign1Source.SelectedItem;

            cboAlign1Grade.Items.Clear();

            tmpIndex = mainVM.LoadExpectationGrade(selectedItem.Txt_source);

            foreach (LbcExpectationIndex ex in tmpIndex)
            {
                bool unique = true;

                //Only add unique items
                foreach (LbcExpectationIndex item in cboAlign1Grade.Items)
                {
                    if (item.Txt_grade == ex.Txt_grade)
                    {
                        unique = false;
                    }
                }

                if (unique == true)
                {
                    cboAlign1Grade.Items.Add(ex);
                }
            }
        }

        private void cboAlign1Grade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			cboAlign1Area.Items.Clear();
			cboAlign1Course.Items.Clear();
			cboAlign1Version.Items.Clear();
			
            if (cboAlign1Grade.SelectedItem != null)
            {
                UpdateCourseAlignmentArea();
            }
        }

        /// <summary>
        /// Update the Grade Combo Box for the course area
        /// </summary>
        private void UpdateCourseAlignmentArea()
        {
            ObservableCollection<LbcExpectationIndex> tmpIndex = new ObservableCollection<LbcExpectationIndex>();
            LbcExpectationIndex selectedItem = (LbcExpectationIndex)cboAlign1Grade.SelectedItem;

            cboAlign1Area.Items.Clear();

            tmpIndex = mainVM.LoadExpectationArea(selectedItem.Txt_source, selectedItem.Txt_grade);

            foreach (LbcExpectationIndex ex in tmpIndex)
            {
                bool unique = true;

                //Only add unique items
                foreach (LbcExpectationIndex item in cboAlign1Area.Items)
                {
                    if (item.Txt_area == ex.Txt_area)
                    {
                        unique = false;
                    }
                }

                if (unique == true)
                {
                    cboAlign1Area.Items.Add(ex);
                }
            }
        }

        private void cboAlign1Area_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			cboAlign1Course.Items.Clear();
			cboAlign1Version.Items.Clear();
			
            if (cboAlign1Area.SelectedItem != null)
            {
                UpdateCourseAlignmentCourse();
            }
        }

        private void UpdateCourseAlignmentCourse()
        {
            ObservableCollection<LbcExpectationIndex> tmpIndex = new ObservableCollection<LbcExpectationIndex>();
            LbcExpectationIndex selectedItem = (LbcExpectationIndex)cboAlign1Area.SelectedItem;

            cboAlign1Course.Items.Clear();

            tmpIndex = mainVM.LoadExpectationCourse(selectedItem.Txt_source, selectedItem.Txt_grade, selectedItem.Txt_area);

            foreach (LbcExpectationIndex ex in tmpIndex)
            {
                bool unique = true;

                //Only add unique items
                foreach (LbcExpectationIndex item in cboAlign1Course.Items)
                {
                    if (item.Txt_course == ex.Txt_course)
                    {
                        unique = false;
                    }
                }

                if (unique == true)
                {
                    cboAlign1Course.Items.Add(ex);
                }
            }
        }

        private void cboAlign1Course_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			cboAlign1Version.Items.Clear();
			
            if (cboAlign1Course.SelectedItem != null)
            {
                UpdateCourseAlignmentVersion();
            }
        }

        private void UpdateCourseAlignmentVersion()
        {
            ObservableCollection<LbcExpectationIndex> tmpIndex = new ObservableCollection<LbcExpectationIndex>();
            LbcExpectationIndex selectedItem = (LbcExpectationIndex)cboAlign1Course.SelectedItem;

            cboAlign1Version.Items.Clear();

            tmpIndex = mainVM.LoadExpectationVersion(selectedItem.Txt_source, selectedItem.Txt_grade, selectedItem.Txt_area, selectedItem.Txt_course);

            foreach (LbcExpectationIndex ex in tmpIndex)
            {
                bool unique = true;

                //Only add unique items
                foreach (LbcExpectationIndex item in cboAlign1Version.Items)
                {
                    if (item.Txt_course == ex.Txt_course)
                    {
                        unique = false;
                    }
                }

                if (unique == true)
                {
                    cboAlign1Version.Items.Add(ex);
                }
            }
        }

        private void cboAlign1Version_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Don't do anything if we're loading the course from the curriculum tree
            if (loadingCourse)
                return;


            if (cboAlign1Version.SelectedItem != null)
            {
                LbcExpectationIndex selectedItem = (LbcExpectationIndex)cboAlign1Course.SelectedItem;

                lblAlign1VersionInfo.Content = selectedItem.Description;

                if (mainVM.UpdateCourseAlignment(selectedItem, courseVM.CurCourse))
                {
                    //The course alignment was updated successfully
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "Course",
                        "Course alignment updated",
                        courseVM.CurCourse.Name + " is now aligned to the selected expectation set."));
                }
                else
                {
                    //Failed to update the course alignment
                    mainVM.InformationMessages.Insert(0, new LBC.Helper.Information(
                        "Warning",
                        "Failed to update course alignment.",
                        "An error occured while update the course alignment. If the problem persists, please restart the program."));
                }
            }
        }

        /// <summary>
        /// Filter the user list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUserFilter_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        /// Create a new user account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMainAddUser_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Display the selected user's information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
