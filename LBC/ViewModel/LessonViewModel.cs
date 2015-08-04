using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using lbcLibrary;
using LBC.DataModels;
using LBC.Interfaces;

namespace LBC.ViewModel
{
    class LessonViewModel : INotifyPropertyChanged
    {
        // =======================================================
        // Events
        // =======================================================
        public event PropertyChangedEventHandler PropertyChanged;

        // =======================================================
        // Private Variables
        // =======================================================
        private Lesson _CurLesson = new Lesson();
        private List<int> _NumDays = new List<int>();
        private ILessonModel lsnContext = new LessonModel();
        private MyDescription myWorkspaceDescription = new MyDescription();
        private MyMaterial myWorkspaceMaterials = new MyMaterial();
        private MyModification myWorkspaceModifications = new MyModification();
        private MyMHLE myWorkspaceAnticipatorySet = new MyMHLE();
        private MyMHLE myWorkspacePurpose = new MyMHLE();
        private MyMHLE myWorkspaceInput = new MyMHLE();
        private MyMHLE myWorkspaceModel = new MyMHLE();
        private MyMHLE myWorkspaceCheckForUnderstanding = new MyMHLE();
        private MyMHLE myWorkspaceGuidedPractice = new MyMHLE();
        private MyMHLE myWorkspaceClosure = new MyMHLE();
        private MyMHLE myWorkspaceIndependentPractice = new MyMHLE();

        // =======================================================
        // Public Properties
        // =======================================================       
        public Lesson CurLesson
        {
            get
            {
                return _CurLesson;
            }
            set
            {
                if (value != null)
                {
                    _CurLesson = value;

                    GetLessonWorkspaceFields(Helper.Session.CurUser.ID);
                    NotifyPropertyChanged("CurLesson");
                }
            }
        }
        public String MyWorkspaceDescription
        {
            get
            {
                return myWorkspaceDescription.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspaceDescription.Description = value;
                    CurLesson.LessonMyDescriptions.Where(l => l.UserID == Helper.Session.CurUser.ID).First().MyDescription = myWorkspaceDescription;
                }
            }
        }
        public String MyWorkspaceMaterials
        {
            get
            {
                return myWorkspaceMaterials.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspaceMaterials.Description = value;
                    CurLesson.LessonMyMaterials.Where(l => l.UserID == Helper.Session.CurUser.ID).First().MyMaterial = myWorkspaceMaterials;
                }
            }
        }
        public String MyWorkspaceModifications
        {
            get
            {
                return myWorkspaceModifications.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspaceModifications.Description = value;
                    CurLesson.LessonMyModifications.Where(l => l.UserID == Helper.Session.CurUser.ID).First().MyModification = myWorkspaceModifications;
                }
            }
        }
        public String MyAnticipatorySet
        {
            get
            {
                return myWorkspaceAnticipatorySet.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspaceAnticipatorySet.Description = value;
                    CurLesson.LessonMyMHLEs.Where(l => l.UserID == Helper.Session.CurUser.ID && l.MyMHLE.Area == "AS").First().MyMHLE = myWorkspaceAnticipatorySet;
                }
            }
        }
        public String MyPurpose
        {
            get
            {
                return myWorkspacePurpose.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspacePurpose.Description = value;
                    CurLesson.LessonMyMHLEs.Where(l => l.UserID == Helper.Session.CurUser.ID && l.MyMHLE.Area == "Purpose").First().MyMHLE = myWorkspacePurpose;
                }
            }
        }
        public String MyInput
        {
            get
            {
                return myWorkspaceInput.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspaceInput.Description = value;
                    CurLesson.LessonMyMHLEs.Where(l => l.UserID == Helper.Session.CurUser.ID && l.MyMHLE.Area == "Input").First().MyMHLE = myWorkspaceInput;
                }
            }
        }
        public String MyModel
        {
            get
            {
                return myWorkspaceModel.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspaceModel.Description = value;
                    CurLesson.LessonMyMHLEs.Where(l => l.UserID == Helper.Session.CurUser.ID && l.MyMHLE.Area == "Model").First().MyMHLE = myWorkspaceModel;
                }
            }
        }
        public String MyCheckForUnderstanding
        {
            get
            {
                return myWorkspaceCheckForUnderstanding.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspaceCheckForUnderstanding.Description = value;
                    CurLesson.LessonMyMHLEs.Where(l => l.UserID == Helper.Session.CurUser.ID && l.MyMHLE.Area == "CFU").First().MyMHLE = myWorkspaceCheckForUnderstanding;
                }
            }
        }
        public String MyGuidedPractice
        {
            get
            {
                return myWorkspaceGuidedPractice.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspaceGuidedPractice.Description = value;
                    CurLesson.LessonMyMHLEs.Where(l => l.UserID == Helper.Session.CurUser.ID && l.MyMHLE.Area == "GP").First().MyMHLE = myWorkspaceGuidedPractice;
                }
            }
        }
        public String MyClosure
        {
            get
            {
                return myWorkspaceClosure.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspaceClosure.Description = value;
                    CurLesson.LessonMyMHLEs.Where(l => l.UserID == Helper.Session.CurUser.ID && l.MyMHLE.Area == "Closure").First().MyMHLE = myWorkspaceClosure;
                }
            }
        }
        public String MyIndependentPractice
        {
            get
            {
                return myWorkspaceIndependentPractice.Description;
            }
            set
            {
                if (value != null)
                {
                    myWorkspaceIndependentPractice.Description = value;
                    CurLesson.LessonMyMHLEs.Where(l => l.UserID == Helper.Session.CurUser.ID && l.MyMHLE.Area == "IP").First().MyMHLE = myWorkspaceIndependentPractice;
                }
            }
        }

        private void GetLessonWorkspaceFields(Guid userID)
        {
            //My Description
            try
            {
                IEnumerable<LessonMyDescription> query = CurLesson.LessonMyDescriptions.Where(l => l.UserID == userID);

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyDescription newDesc = new MyDescription();
                    LessonMyDescription newLsnDesc = new LessonMyDescription();

                    newDesc.ID = Guid.NewGuid();
                    newDesc.Description = string.Empty;
                    newLsnDesc.LessonID = CurLesson.ID;
                    newLsnDesc.DescriptionID = newDesc.ID;
                    newLsnDesc.UserID = userID;
                    newLsnDesc.MyDescription = newDesc;

                    Helper.Session.DbContext.Add(newDesc);
                    Helper.Session.DbContext.Add(newLsnDesc);
                    CurLesson.LessonMyDescriptions.Add(newLsnDesc);

                    myWorkspaceDescription = newLsnDesc.MyDescription;
                }
                else
                {
                    myWorkspaceDescription = query.First().MyDescription;
                }
            }
            catch{}
            //My Materials
            try
            {
                IEnumerable<LessonMyMaterial> query = CurLesson.LessonMyMaterials.Where(l => l.UserID == userID);

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyMaterial newMaterial = new MyMaterial();
                    LessonMyMaterial newLsnMaterial = new LessonMyMaterial();

                    newMaterial.ID = Guid.NewGuid();
                    newMaterial.Description = string.Empty;
                    newLsnMaterial.LessonID = CurLesson.ID;
                    newLsnMaterial.MaterialID = newMaterial.ID;
                    newLsnMaterial.UserID = userID;
                    newLsnMaterial.MyMaterial = newMaterial;

                    Helper.Session.DbContext.Add(newMaterial);
                    Helper.Session.DbContext.Add(newLsnMaterial);
                    CurLesson.LessonMyMaterials.Add(newLsnMaterial);

                    myWorkspaceMaterials = newLsnMaterial.MyMaterial;
                }
                else
                {
                    myWorkspaceMaterials = query.First().MyMaterial;
                }
            }
            catch { }
            //My Modifications
            try
            {
                IEnumerable<LessonMyModification> query = CurLesson.LessonMyModifications.Where(l => l.UserID == userID);

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyModification newModification = new MyModification();
                    LessonMyModification newLsnModification = new LessonMyModification();

                    newModification.ID = Guid.NewGuid();
                    newModification.Description = string.Empty;
                    newLsnModification.LessonID = CurLesson.ID;
                    newLsnModification.ModificationID = newModification.ID;
                    newLsnModification.UserID = userID;
                    newLsnModification.MyModification = newModification;

                    Helper.Session.DbContext.Add(newModification);
                    Helper.Session.DbContext.Add(newLsnModification);
                    CurLesson.LessonMyModifications.Add(newLsnModification);

                    myWorkspaceModifications = newLsnModification.MyModification;
                }
                else
                {
                    myWorkspaceModifications = query.First().MyModification;
                }
            }
            catch { }
            //My Anticipatory Set
            try
            {
                IEnumerable<LessonMyMHLE> query = CurLesson.LessonMyMHLEs.Where(l => l.UserID == userID && l.MyMHLE.Area == "AS");

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyMHLE newMHLE = new MyMHLE();
                    LessonMyMHLE newLsnMHLE = new LessonMyMHLE();

                    newMHLE.ID = Guid.NewGuid();
                    newMHLE.Description = string.Empty;
                    newMHLE.Area = "AS";
                    newLsnMHLE.LessonID = CurLesson.ID;
                    newLsnMHLE.MHLEID = newMHLE.ID;
                    newLsnMHLE.UserID = userID;
                    newLsnMHLE.MyMHLE = newMHLE;

                    Helper.Session.DbContext.Add(newMHLE);
                    Helper.Session.DbContext.Add(newLsnMHLE);
                    CurLesson.LessonMyMHLEs.Add(newLsnMHLE);

                    myWorkspaceAnticipatorySet = newLsnMHLE.MyMHLE;
                }
                else
                {
                    myWorkspaceAnticipatorySet = query.First().MyMHLE;
                }
            }
            catch { }
            //My Purpose
            try
            {
                IEnumerable<LessonMyMHLE> query = CurLesson.LessonMyMHLEs.Where(l => l.UserID == userID && l.MyMHLE.Area == "Purpose");

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyMHLE newMHLE = new MyMHLE();
                    LessonMyMHLE newLsnMHLE = new LessonMyMHLE();

                    newMHLE.ID = Guid.NewGuid();
                    newMHLE.Description = string.Empty;
                    newMHLE.Area = "Purpose";
                    newLsnMHLE.LessonID = CurLesson.ID;
                    newLsnMHLE.MHLEID = newMHLE.ID;
                    newLsnMHLE.UserID = userID;
                    newLsnMHLE.MyMHLE = newMHLE;

                    Helper.Session.DbContext.Add(newMHLE);
                    Helper.Session.DbContext.Add(newLsnMHLE);
                    CurLesson.LessonMyMHLEs.Add(newLsnMHLE);

                    myWorkspacePurpose = newLsnMHLE.MyMHLE;
                }
                else
                {
                    myWorkspacePurpose = query.First().MyMHLE;
                }
            }
            catch { }
            //Input
            try
            {
                IEnumerable<LessonMyMHLE> query = CurLesson.LessonMyMHLEs.Where(l => l.UserID == userID && l.MyMHLE.Area == "Input");

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyMHLE newMHLE = new MyMHLE();
                    LessonMyMHLE newLsnMHLE = new LessonMyMHLE();

                    newMHLE.ID = Guid.NewGuid();
                    newMHLE.Description = string.Empty;
                    newMHLE.Area = "Input";
                    newLsnMHLE.LessonID = CurLesson.ID;
                    newLsnMHLE.MHLEID = newMHLE.ID;
                    newLsnMHLE.UserID = userID;
                    newLsnMHLE.MyMHLE = newMHLE;

                    Helper.Session.DbContext.Add(newMHLE);
                    Helper.Session.DbContext.Add(newLsnMHLE);
                    CurLesson.LessonMyMHLEs.Add(newLsnMHLE);

                    myWorkspaceInput = newLsnMHLE.MyMHLE;
                }
                else
                {
                    myWorkspaceInput = query.First().MyMHLE;
                }
            }
            catch { }
            //Model
            try
            {
                IEnumerable<LessonMyMHLE> query = CurLesson.LessonMyMHLEs.Where(l => l.UserID == userID && l.MyMHLE.Area == "Model");

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyMHLE newMHLE = new MyMHLE();
                    LessonMyMHLE newLsnMHLE = new LessonMyMHLE();

                    newMHLE.ID = Guid.NewGuid();
                    newMHLE.Description = string.Empty;
                    newMHLE.Area = "Model";
                    newLsnMHLE.LessonID = CurLesson.ID;
                    newLsnMHLE.MHLEID = newMHLE.ID;
                    newLsnMHLE.UserID = userID;
                    newLsnMHLE.MyMHLE = newMHLE;

                    Helper.Session.DbContext.Add(newMHLE);
                    Helper.Session.DbContext.Add(newLsnMHLE);
                    CurLesson.LessonMyMHLEs.Add(newLsnMHLE);

                    myWorkspaceModel = newLsnMHLE.MyMHLE;
                }
                else
                {
                    myWorkspaceModel = query.First().MyMHLE;
                }
            }
            catch { }
            //Check For Understanding
            try
            {
                IEnumerable<LessonMyMHLE> query = CurLesson.LessonMyMHLEs.Where(l => l.UserID == userID && l.MyMHLE.Area == "CFU");

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyMHLE newMHLE = new MyMHLE();
                    LessonMyMHLE newLsnMHLE = new LessonMyMHLE();

                    newMHLE.ID = Guid.NewGuid();
                    newMHLE.Description = string.Empty;
                    newMHLE.Area = "CFU";
                    newLsnMHLE.LessonID = CurLesson.ID;
                    newLsnMHLE.MHLEID = newMHLE.ID;
                    newLsnMHLE.UserID = userID;
                    newLsnMHLE.MyMHLE = newMHLE;

                    Helper.Session.DbContext.Add(newMHLE);
                    Helper.Session.DbContext.Add(newLsnMHLE);
                    CurLesson.LessonMyMHLEs.Add(newLsnMHLE);

                    myWorkspaceCheckForUnderstanding = newLsnMHLE.MyMHLE;
                }
                else
                {
                    myWorkspaceCheckForUnderstanding = query.First().MyMHLE;
                }
            }
            catch { }
            //Guided Practice
            try
            {
                IEnumerable<LessonMyMHLE> query = CurLesson.LessonMyMHLEs.Where(l => l.UserID == userID && l.MyMHLE.Area == "GP");

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyMHLE newMHLE = new MyMHLE();
                    LessonMyMHLE newLsnMHLE = new LessonMyMHLE();

                    newMHLE.ID = Guid.NewGuid();
                    newMHLE.Description = string.Empty;
                    newMHLE.Area = "GP";
                    newLsnMHLE.LessonID = CurLesson.ID;
                    newLsnMHLE.MHLEID = newMHLE.ID;
                    newLsnMHLE.UserID = userID;
                    newLsnMHLE.MyMHLE = newMHLE;

                    Helper.Session.DbContext.Add(newMHLE);
                    Helper.Session.DbContext.Add(newLsnMHLE);
                    CurLesson.LessonMyMHLEs.Add(newLsnMHLE);

                    myWorkspaceGuidedPractice = newLsnMHLE.MyMHLE;
                }
                else
                {
                    myWorkspaceGuidedPractice = query.First().MyMHLE;
                }
            }
            catch { }
            //Closure
            try
            {
                IEnumerable<LessonMyMHLE> query = CurLesson.LessonMyMHLEs.Where(l => l.UserID == userID && l.MyMHLE.Area == "Closure");

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyMHLE newMHLE = new MyMHLE();
                    LessonMyMHLE newLsnMHLE = new LessonMyMHLE();

                    newMHLE.ID = Guid.NewGuid();
                    newMHLE.Description = string.Empty;
                    newMHLE.Area = "Closure";
                    newLsnMHLE.LessonID = CurLesson.ID;
                    newLsnMHLE.MHLEID = newMHLE.ID;
                    newLsnMHLE.UserID = userID;
                    newLsnMHLE.MyMHLE = newMHLE;

                    Helper.Session.DbContext.Add(newMHLE);
                    Helper.Session.DbContext.Add(newLsnMHLE);
                    CurLesson.LessonMyMHLEs.Add(newLsnMHLE);

                    myWorkspaceClosure = newLsnMHLE.MyMHLE;
                }
                else
                {
                    myWorkspaceClosure = query.First().MyMHLE;
                }
            }
            catch { }
            //Independent Practice
            try
            {
                IEnumerable<LessonMyMHLE> query = CurLesson.LessonMyMHLEs.Where(l => l.UserID == userID && l.MyMHLE.Area == "IP");

                //Create the record if it doesn't already exist
                if (query.Count() == 0)
                {
                    MyMHLE newMHLE = new MyMHLE();
                    LessonMyMHLE newLsnMHLE = new LessonMyMHLE();

                    newMHLE.ID = Guid.NewGuid();
                    newMHLE.Description = string.Empty;
                    newMHLE.Area = "IP";
                    newLsnMHLE.LessonID = CurLesson.ID;
                    newLsnMHLE.MHLEID = newMHLE.ID;
                    newLsnMHLE.UserID = userID;
                    newLsnMHLE.MyMHLE = newMHLE;

                    Helper.Session.DbContext.Add(newMHLE);
                    Helper.Session.DbContext.Add(newLsnMHLE);
                    CurLesson.LessonMyMHLEs.Add(newLsnMHLE);

                    myWorkspaceIndependentPractice = newLsnMHLE.MyMHLE;
                }
                else
                {
                    myWorkspaceIndependentPractice = query.First().MyMHLE;
                }
            }
            catch { }
        }

        public List<int> NumberOfDays
        {
            get
            {
                return _NumDays;
            }
        }
        public string MyDescription
        {
            get
            {
                return CurLesson.LessonMyDescriptions.Where(
                    l => l.UserID == Helper.Session.CurUser.ID).First().MyDescription.Description;
            }
            set
            {

            }
        }


        // =======================================================
        // Methods
        // =======================================================

        /// <summary>
        /// LessonViewModel constructor
        /// </summary>
        public LessonViewModel()
        {

            //Initialize NumDays
            _NumDays = new List<int>();
            for (int i = 1; i <= 30; i++)
            {
                _NumDays.Add(i);
            }
        }


        /// <summary>
        /// Create a new lesson in the specified unit
        /// </summary>
        /// <param name="unt">The unit to create the lesson in</4param>
        /// <returns></returns>
        public bool CreateNewLesson(Unit unt)
        {
            try
            {
                Lesson newLesson = new Lesson();

                newLesson.ID = Guid.NewGuid();
                newLesson.UnitID = unt.ID;
                newLesson.Unit = unt;
                newLesson.LessonName = "New Lesson";
                newLesson.LessonNum = (unt.Lessons.Count + 1);

                unt.Lessons.Add(newLesson);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete the specified lesson
        /// </summary>
        /// <param name="lsn">The lesson to be deleted</param>
        /// <returns></returns>
        public bool DeleteLesson(Lesson lsn)
        {
            try
            {
                return lsn.Unit.Lessons.Remove(lsn);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Move a lesson into a unit
        /// </summary>
        /// <param name="lsn">The lesson to move</param>
        /// <param name="newUnt">The unit that the lesson is going into</param>
        /// <returns></returns>
        public bool MoveLesson(Lesson lsn, Unit newUnt)
        {
            try
            {
                lsn.UnitID = newUnt.ID;
                lsn.Unit = newUnt;

                //Remove the lesson from its current unit
                lsn.Unit.Lessons.Remove(lsn);

                //Move the lesson into its new unit
                newUnt.Lessons.Add(lsn);

                return true;
            }
            catch
            {
                //something went wrong
                return false;
            }
        }

        /// <summary>
        /// Add a document to the current lesson
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public Document AddDocument(string filename, string filepath)
        {
            try
            {
                Document newDoc = new Document();
            
                newDoc.ID = Guid.NewGuid();
                newDoc.Filename = filename;
                newDoc.FilePath = filepath;

                newDoc.Lessons.Add(CurLesson);
                CurLesson.Documents.Add(newDoc);

                NotifyPropertyChanged("Documents");

                return newDoc;
            }
            catch
            {
                //An error occured while creating the new document
                return null;
            }
        }

        public bool DeleteDocument(Document doc)
        {
            try
            {
                CurLesson.Documents.Remove(doc);
                Helper.Session.DbContext.Delete(doc);

                return true;
            }
            catch
            {
                //An error occured while deleted the document record
                return false;
            }
        }

        public bool DeleteExpectation(LessonExpectation expectation)
        {
            try
            {
                CurLesson.LessonExpectations.Remove(expectation);

                try
                {
                    Helper.Session.DbContext.Delete(expectation);
                }
                catch
                {
                    // This error is generally caused by a user deleting an item
                    // before it has been saved to the database.
                    // Because of this, we can ignore this error
                }

                return true;
            }
            catch
            {
                //An error occured while deleted the document record
                return false;
            }
        }

        /// <summary>
        /// Create a lesson expectation for the given lesson and expectation if one doesn't already exist in the current lesson
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public LessonExpectation AddLessonExpectation(Expectation exp)
        {
            LessonExpectation lsnExpectation = new LessonExpectation();

            try
            {
                //Make sure the expectation hasn't already been added to the lesson
                if (CurLesson.LessonExpectations != null)
                {
                    foreach (LessonExpectation lsnExp in CurLesson.LessonExpectations)
                    {
                        if (lsnExp.SENumber == exp.Senumber)
                            return null;
                    }
                }

                lsnExpectation.Expectation = exp;
                lsnExpectation.Lesson = CurLesson;
                lsnExpectation.LessonDOK = 0;
                lsnExpectation.LessonID = CurLesson.ID;
                lsnExpectation.SENumber = exp.Senumber;
                lsnExpectation.UnitDOK = 0;

                return lsnExpectation;
            }
            catch
            {
                //An error occured
                return null;
            }
            finally
            {
                lsnExpectation = null;
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
