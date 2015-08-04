using System.Collections.ObjectModel;
using lbcLibrary;
using acManagement;
using System.Collections.Generic;
using System.Linq;
using LBC.Interfaces;
using LBC.Helper;
using System.Windows;
using System;

namespace LBC.DataModels
{
    class ExpectationModel : IExpectationModel
    {
        /// <summary>
        /// Loads the complete expectation index table from the database and stores it in Session.ExpectationList
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<LbcExpectationIndex> LoadExpectationIndex()
        {
            ObservableCollection<LbcExpectationIndex> allExpectationIndicies = new ObservableCollection<LbcExpectationIndex>();
            IQueryable<LbcExpectationIndex> loadedExpectationIndicies;

            try
            {
                loadedExpectationIndicies = Session.ManagementContext.LbcExpectationIndices;

                foreach (LbcExpectationIndex ex in loadedExpectationIndicies)
                {
                    allExpectationIndicies.Add(ex);
                }

                return allExpectationIndicies;
            }
            catch
            {
                return null;
            }
            finally
            {
                allExpectationIndicies = null;
                loadedExpectationIndicies = null;
            }
        }

        public ObservableCollection<LbcExpectationIndex> GetExpectationIndexGrade(ref ObservableCollection<LbcExpectationIndex> expectationList, string txt_Source)
        {
            List<LbcExpectationIndex> tmpList = new List<LbcExpectationIndex>();
            ObservableCollection<LbcExpectationIndex> indexList = new ObservableCollection<LbcExpectationIndex>();

            try
            {
                tmpList = (from expList in expectationList where expList.Txt_source == txt_Source select expList).ToList();

                foreach (LbcExpectationIndex exp in tmpList)
                {
                    indexList.Add(exp);
                }

                return indexList;
            }
            catch
            {
                return null;
            }
            finally
            {
                tmpList = null;
                indexList = null;
            }
        }

        public ObservableCollection<LbcExpectationIndex> GetExpectationIndexArea(ref ObservableCollection<LbcExpectationIndex> expectationList, string txt_Source, string txt_Grade)
        {
            List<LbcExpectationIndex> tmpList = new List<LbcExpectationIndex>();
            ObservableCollection<LbcExpectationIndex> indexList = new ObservableCollection<LbcExpectationIndex>();

            try
            {
                tmpList = (from expList in expectationList where expList.Txt_source == txt_Source && expList.Txt_grade == txt_Grade select expList).ToList();

                foreach (LbcExpectationIndex exp in tmpList)
                {
                    indexList.Add(exp);
                }

                return indexList;
            }
            catch
            {
                return null;
            }
            finally
            {
                tmpList = null;
                indexList = null;
            }
        }

        public ObservableCollection<LbcExpectationIndex> GetExpectationIndexCourse(ref ObservableCollection<LbcExpectationIndex> expectationList, string txt_Source, string txt_Grade, string txt_Area)
        {
            List<LbcExpectationIndex> tmpList = new List<LbcExpectationIndex>();
            ObservableCollection<LbcExpectationIndex> indexList = new ObservableCollection<LbcExpectationIndex>();

            try
            {
                tmpList = (from expList in expectationList where expList.Txt_source == txt_Source && expList.Txt_grade == txt_Grade && expList.Txt_area == txt_Area select expList).ToList();

                foreach (LbcExpectationIndex exp in tmpList)
                {
                    indexList.Add(exp);
                }

                return indexList;
            }
            catch
            {
                return null;
            }
            finally
            {
                tmpList = null;
                indexList = null;
            }
        }

        public ObservableCollection<LbcExpectationIndex> GetExpectationIndexVersion(ref ObservableCollection<LbcExpectationIndex> expectationList, string txt_Source, string txt_Grade, string txt_Area, string txt_course)
        {
            List<LbcExpectationIndex> tmpList = new List<LbcExpectationIndex>();
            ObservableCollection<LbcExpectationIndex> indexList = new ObservableCollection<LbcExpectationIndex>();

            try
            {
                tmpList = (from expList in expectationList where expList.Txt_source == txt_Source && expList.Txt_grade == txt_Grade && expList.Txt_course == txt_course select expList).ToList();

                foreach (LbcExpectationIndex exp in tmpList)
                {
                    indexList.Add(exp);
                }

                return indexList;
            }
            catch
            {
                return null;
            }
            finally
            {
                tmpList = null;
                indexList = null;
            }
        }

        public ObservableCollection<LbcExpectationIndex> GetExpectationSource()
        {
            ObservableCollection<LbcExpectationIndex> allExpectationIndicies = new ObservableCollection<LbcExpectationIndex>();
            IQueryable<LbcExpectationIndex> loadedExpectationIndicies;

            try
            {
                loadedExpectationIndicies = Session.ManagementContext.LbcExpectationIndices;

                foreach (LbcExpectationIndex ex in loadedExpectationIndicies)
                {
                    allExpectationIndicies.Add(ex);
                }

                return allExpectationIndicies;
            }
            catch
            {
                return null;
            }
            finally
            {
                allExpectationIndicies = null;
                loadedExpectationIndicies = null;
            }
        }

        public ObservableCollection<Expectation> LoadCourseExpectations(long? ExpectationStart, long? ExpectationStop)
        {
            ObservableCollection<Expectation> tmpExpectations = new ObservableCollection<Expectation>();
            List<Expectation> expectationList = new List<Expectation>();

            expectationList = Session.DbContext.Expectations.Where(e => e.Senumber >= ExpectationStart && e.Senumber <= ExpectationStop).ToList();

            foreach (Expectation exp in expectationList)
            {
                tmpExpectations.Add(exp);
            }

            return tmpExpectations;
        }

        public bool UpdateCourseExpectations(LbcExpectationIndex newIndex, Course crs, ref ObservableCollection<CourseMeta> metaData)
        {
            CourseMeta tmpMeta;

            try
            {
                //Update the course's expectation start and stop code
                crs.ExpectationCodeStart = long.Parse(newIndex.CodeStart);
                crs.ExpectationCodeStop = long.Parse(newIndex.CodeEnd);

                //Find the meta data record for the current course
                tmpMeta = metaData.Where(m => m.CourseID == crs.ID).First();

                //Load the new expectations for the given course
                if (crs.ExpectationCodeStart != null && crs.ExpectationCodeStop != null)
                {
                    tmpMeta.CourseExpectations = LoadCourseExpectations(crs.ExpectationCodeStart, crs.ExpectationCodeStop);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LoadCurriculumExpectations(ObservableCollection<Course> Curriculum, ObservableCollection<CourseMeta> MetaData)
        {
            CourseMeta tmpMeta;

            try
            {
                foreach (Course crs in Curriculum)
                {
                    try
                    {
                        //Find the meta data record for the current course
                        tmpMeta = MetaData.Where(m => m.CourseID == crs.ID).First();

                        //Load the expectations for the given course
                        if (crs.ExpectationCodeStart != null && crs.ExpectationCodeStop != null)
                        {
                            tmpMeta.CourseExpectations = LoadCourseExpectations(crs.ExpectationCodeStart, crs.ExpectationCodeStop);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Resume Next
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
