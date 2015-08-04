using Itenso.Rtf;
using Itenso.Rtf.Converter.Html;
using Itenso.Rtf.Support;
using lbcLibrary;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace lbcDataConverter.ViewModels
{
    class MainViewModel
    {
        // =======================================================
        // Private Variables
        // =======================================================
        //private LbcDbContext dbContext = new LbcDbContext("data source=204.51.98.176;initial catalog=lbc_ava;persist security info=True;user id=lbc_app;password=LbWv09;multipleactiveresultsets=True");
        private LbcDbContext dbContext = new LbcDbContext("data source=Win-Server01;initial catalog=lbc_developer;persist security info=True;user id=sa;password=Tracker05;multipleactiveresultsets=True");
        private List<Course> _Courses;
        private List<MyMHLE> _MyMHLEs;
        private List<MyDescription> _MyDescriptions;
        private List<MyMaterial> _MyMaterials;
        private List<MyModification> _MyModifications;

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public bool LoadCurriculum()
        {
            try
            {
                _Courses = dbContext.Courses.ToList();
                _MyMHLEs = dbContext.MyMHLEs.ToList();
                _MyDescriptions = dbContext.MyDescriptions.ToList();
                _MyMaterials = dbContext.MyMaterials.ToList();
                _MyModifications = dbContext.MyModifications.ToList();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public bool ConvertCourseFields()
        {
            IRtfDocument crsPhilosophy;
            IRtfDocument crsRationale;
            IRtfDocument crsDescription;
            RtfHtmlConverter htmlConverter;


                foreach (Course crs in _Courses)
                {
                    crs.Grade = ConvertCourseGrade(crs.Grade);

                    try
                    {
                        //Convert Philosophy
                        if (crs.Philosophy != "" && crs.Philosophy != null)
                        {
                            crsPhilosophy = RtfInterpreterTool.BuildDoc(crs.Philosophy);
                            htmlConverter = new RtfHtmlConverter(crsPhilosophy);
                            crs.Philosophy = htmlConverter.Convert();
                        }
                    }
                    catch (Itenso.Rtf.RtfStructureException ex)
                    {
                        //Resume Next
                    }
                    try
                    {
                        //Convert Rationale
                        if (crs.Rationale != "" && crs.Rationale != null)
                        {
                            crsRationale = RtfInterpreterTool.BuildDoc(crs.Rationale);
                            htmlConverter = new RtfHtmlConverter(crsRationale);
                            crs.Rationale = htmlConverter.Convert();
                        }
                    }
                    catch (Itenso.Rtf.RtfStructureException ex)
                    {
                        //Resume Next
                    }
                    try
                    {
                        //Convert Description
                        if (crs.Description != "" && crs.Description != null)
                        {
                            crsDescription = RtfInterpreterTool.BuildDoc(crs.Description);
                            htmlConverter = new RtfHtmlConverter(crsDescription);
                            crs.Description = htmlConverter.Convert();
                        }
                    }
                    catch (Itenso.Rtf.RtfStructureException ex)
                    {
                        //Resume Next
                    }

                }

                SaveChanges();

                return true;
        }

        public bool ConvertMyWorkspace()
        {
            IRtfDocument myDescription;
            IRtfDocument myMaterial;
            IRtfDocument myMHLE;
            IRtfDocument myModification;
            RtfHtmlConverter htmlConverter;


            //Convert MyDescription table
            foreach (MyDescription desc in _MyDescriptions)
            {
                try
                {
                    if (desc.Description != "" && desc.Description != null)
                    {
                        myDescription = RtfInterpreterTool.BuildDoc(desc.Description);
                        htmlConverter = new RtfHtmlConverter(myDescription);
                        desc.Description = htmlConverter.Convert();
                    }
                }
                catch (Itenso.Rtf.RtfStructureException ex)
                {
                    //Resume Next
                }
            }

            //Convert MyMaterial table
            foreach (MyMaterial mat in _MyMaterials)
            {
                try
                {
                    if (mat.Description != "" && mat.Description != null)
                    {
                        myDescription = RtfInterpreterTool.BuildDoc(mat.Description);
                        htmlConverter = new RtfHtmlConverter(myDescription);
                        mat.Description = htmlConverter.Convert();
                    }
                }
                catch (Itenso.Rtf.RtfStructureException ex)
                {
                    //Resume Next
                }
            }

            //Convert MyMHLE table
            foreach (MyMHLE mhle in _MyMHLEs)
            {
                try
                {
                    if (mhle.Description != "" && mhle.Description != null)
                    {
                        myDescription = RtfInterpreterTool.BuildDoc(mhle.Description);
                        htmlConverter = new RtfHtmlConverter(myDescription);
                        mhle.Description = htmlConverter.Convert();
                    }
                }
                catch (Itenso.Rtf.RtfStructureException ex)
                {
                    //Resume Next
                }
            }

            //Convert MyModification table
            foreach (MyModification mod in _MyModifications)
            {
                try
                {
                    if (mod.Description != "" && mod.Description != null)
                    {
                        myDescription = RtfInterpreterTool.BuildDoc(mod.Description);
                        htmlConverter = new RtfHtmlConverter(myDescription);
                        mod.Description = htmlConverter.Convert();
                    }
                }
                catch (Itenso.Rtf.RtfStructureException ex)
                {
                    //Resume Next
                }
            }

            SaveChanges();

            return true;
        }

        private string ConvertCourseGrade(string oldGrade)
        {
            switch (oldGrade)
            {
                case "PK":
                    return "Preschool";
                case "K":
                    return "Kindergarten";
                case "1":
                    return "1st Grade";
                case "2":
                    return "2nd Grade";
                case "3":
                    return "3rd Grade";
                case "4":
                    return "4th Grade";
                case "5":
                    return "5th Grade";
                case "6":
                    return "6th Grade";
                case "7":
                    return "7th Grade";
                case "8":
                    return "8th Grade";
                case "9":
                    return "9th Grade";
                case "10":
                    return "10th Grade";
                case "11":
                    return "11th Grade";
                case "12":
                    return "12th Grade";
                case "U":
                    return "Undefined";

            }

            return null;
        }

        public bool ConvertLessonFields()
        {
            IRtfDocument lsnDescription;
            IRtfDocument lsnModifications;
            IRtfDocument lsnMaterials;
            IRtfDocument lsnMHLE_AS;
            IRtfDocument lsnMHLE_Purpose;
            IRtfDocument lsnMHLE_Input;
            IRtfDocument lsnMHLE_Model;
            IRtfDocument lsnMHLE_CFU;
            IRtfDocument lsnMHLE_GP;
            IRtfDocument lsnMHLE_Closure;
            IRtfDocument lsnMHLE_IP;
            RtfHtmlConverter htmlConverter;

            try
            {
                foreach (Course crs in _Courses)
                {
                    foreach (Unit unt in crs.Units)
                    {
                        foreach (Lesson lsn in unt.Lessons)
                        {
                            try
                            {
                                //Convert Description
                                if (lsn.Description != "" && lsn.Description != null)
                                {
                                    lsnDescription = RtfInterpreterTool.BuildDoc(lsn.Description);
                                    htmlConverter = new RtfHtmlConverter(lsnDescription);
                                    lsn.Description = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert Modifications
                                if (lsn.Modifications != "" && lsn.Modifications != null)
                                {
                                    lsnModifications = RtfInterpreterTool.BuildDoc(lsn.Modifications);
                                    htmlConverter = new RtfHtmlConverter(lsnModifications);
                                    lsn.Modifications = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert Materials
                                if (lsn.Materials != "" && lsn.Materials != null)
                                {
                                    lsnMaterials = RtfInterpreterTool.BuildDoc(lsn.Materials);
                                    htmlConverter = new RtfHtmlConverter(lsnMaterials);
                                    lsn.Materials = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_AS
                                if (lsn.MHLEAS != "" && lsn.MHLEAS != null)
                                {
                                    lsnMHLE_AS = RtfInterpreterTool.BuildDoc(lsn.MHLEAS);
                                    htmlConverter = new RtfHtmlConverter(lsnMHLE_AS);
                                    lsn.MHLEAS = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_Purpose
                                if (lsn.MHLEPurpose != "" && lsn.MHLEPurpose != null)
                                {
                                    lsnMHLE_Purpose = RtfInterpreterTool.BuildDoc(lsn.MHLEPurpose);
                                    htmlConverter = new RtfHtmlConverter(lsnMHLE_Purpose);
                                    lsn.MHLEPurpose = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_Input
                                if (lsn.MHLEInput != "" && lsn.MHLEInput != null)
                                {
                                    lsnMHLE_Input = RtfInterpreterTool.BuildDoc(lsn.MHLEInput);
                                    htmlConverter = new RtfHtmlConverter(lsnMHLE_Input);
                                    lsn.MHLEInput = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_Model
                                if (lsn.MHLEModel != "" && lsn.MHLEModel != null)
                                {
                                    lsnMHLE_Model = RtfInterpreterTool.BuildDoc(lsn.MHLEModel);
                                    htmlConverter = new RtfHtmlConverter(lsnMHLE_Model);
                                    lsn.MHLEModel = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_CFU
                                if (lsn.MHLECFU != "" && lsn.MHLECFU != null)
                                {
                                    lsnMHLE_CFU = RtfInterpreterTool.BuildDoc(lsn.MHLECFU);
                                    htmlConverter = new RtfHtmlConverter(lsnMHLE_CFU);
                                    lsn.MHLECFU = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_GP
                                if (lsn.MHLEGP != "" && lsn.MHLEGP != null)
                                {
                                    lsnMHLE_GP = RtfInterpreterTool.BuildDoc(lsn.MHLEGP);
                                    htmlConverter = new RtfHtmlConverter(lsnMHLE_GP);
                                    lsn.MHLEGP = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_Closure
                                if (lsn.MHLEClosure != "" && lsn.MHLEClosure != null)
                                {
                                    lsnMHLE_Closure = RtfInterpreterTool.BuildDoc(lsn.MHLEClosure);
                                    htmlConverter = new RtfHtmlConverter(lsnMHLE_Closure);
                                    lsn.MHLEClosure = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_IP
                                if (lsn.MHLEIP != "" && lsn.MHLEIP != null)
                                {
                                    lsnMHLE_IP = RtfInterpreterTool.BuildDoc(lsn.MHLEIP);
                                    htmlConverter = new RtfHtmlConverter(lsnMHLE_IP);
                                    lsn.MHLEIP = htmlConverter.Convert();
                                }
                            }
                            catch (Itenso.Rtf.RtfStructureException ex)
                            {
                                //Resume Next
                            }

                            //Convert Lesson Objectives
                            foreach (LessonObjective obj in lsn.LessonObjectives)
                            {
                                lsn.DailyObjectives += obj.Number + ". " + obj.Description + "     ";
                            }
                        }
                    }
                }



                SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public bool ConvertHTMLFields()
        {
            try
            {
                foreach (Course crs in _Courses)
                {
                    try
                    {
                        //Convert Philosophy
                        if (crs.Philosophy != "" && crs.Philosophy != null)
                        {
                            crs.Philosophy = DeleteHTMLToTag("<body>", crs.Philosophy);
                            crs.Philosophy = DeleteHTMLAfterTag("</body>", crs.Philosophy);
                        }
                    }
                    catch
                    {
                        //Resume Next
                    }
                    try
                    {
                        //Convert Rationale
                        if (crs.Rationale != "" && crs.Rationale != null)
                        {
                            crs.Rationale = DeleteHTMLToTag("<body>", crs.Rationale);
                            crs.Rationale = DeleteHTMLAfterTag("</body>", crs.Rationale);
                        }
                    }
                    catch
                    {
                        //Resume Next
                    }
                    try
                    {
                        //Convert Description
                        if (crs.Description != "" && crs.Description != null)
                        {
                            crs.Description = DeleteHTMLToTag("<body>", crs.Description);
                            crs.Description = DeleteHTMLAfterTag("</body>", crs.Description);
                        }
                    }
                    catch
                    {
                        //Resume Next
                    }


                    foreach (Unit unt in crs.Units)
                    {
                        foreach (Lesson lsn in unt.Lessons)
                        {
                            try
                            {
                                //Convert Description
                                if (lsn.Description != "" && lsn.Description != null)
                                {
                                    lsn.Description = DeleteHTMLToTag("<body>", lsn.Description);
                                    lsn.Description = DeleteHTMLAfterTag("</body>", lsn.Description);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert Modifications
                                if (lsn.Modifications != "" && lsn.Modifications != null)
                                {
                                    lsn.Modifications = DeleteHTMLToTag("<body>", lsn.Modifications);
                                    lsn.Modifications = DeleteHTMLAfterTag("</body>", lsn.Modifications);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert Materials
                                if (lsn.Materials != "" && lsn.Materials != null)
                                {
                                    lsn.Materials = DeleteHTMLToTag("<body>", lsn.Materials);
                                    lsn.Materials = DeleteHTMLAfterTag("</body>", lsn.Materials);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_AS
                                if (lsn.MHLEAS != "" && lsn.MHLEAS != null)
                                {
                                    lsn.MHLEAS = DeleteHTMLToTag("<body>", lsn.MHLEAS);
                                    lsn.MHLEAS = DeleteHTMLAfterTag("</body>", lsn.MHLEAS);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_Purpose
                                if (lsn.MHLEPurpose != "" && lsn.MHLEPurpose != null)
                                {
                                    lsn.MHLEPurpose = DeleteHTMLToTag("<body>", lsn.MHLEPurpose);
                                    lsn.MHLEPurpose = DeleteHTMLAfterTag("</body>", lsn.MHLEPurpose);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_Input
                                if (lsn.MHLEInput != "" && lsn.MHLEInput != null)
                                {
                                    lsn.MHLEInput = DeleteHTMLToTag("<body>", lsn.MHLEInput);
                                    lsn.MHLEInput = DeleteHTMLAfterTag("</body>", lsn.MHLEInput);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_Model
                                if (lsn.MHLEModel != "" && lsn.MHLEModel != null)
                                {
                                    lsn.MHLEModel = DeleteHTMLToTag("<body>", lsn.MHLEModel);
                                    lsn.MHLEModel = DeleteHTMLAfterTag("</body>", lsn.MHLEModel);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_CFU
                                if (lsn.MHLECFU != "" && lsn.MHLECFU != null)
                                {
                                    lsn.MHLECFU = DeleteHTMLToTag("<body>", lsn.MHLECFU);
                                    lsn.MHLECFU = DeleteHTMLAfterTag("</body>", lsn.MHLECFU);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_GP
                                if (lsn.MHLEGP != "" && lsn.MHLEGP != null)
                                {
                                    lsn.MHLEGP = DeleteHTMLToTag("<body>", lsn.MHLEGP);
                                    lsn.MHLEGP = DeleteHTMLAfterTag("</body>", lsn.MHLEGP);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_Closure
                                if (lsn.MHLEClosure != "" && lsn.MHLEClosure != null)
                                {
                                    lsn.MHLEClosure = DeleteHTMLToTag("<body>", lsn.MHLEClosure);
                                    lsn.MHLEClosure = DeleteHTMLAfterTag("</body>", lsn.MHLEClosure);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                            try
                            {
                                //Convert MHLE_IP
                                if (lsn.MHLEIP != "" && lsn.MHLEIP != null)
                                {
                                    lsn.MHLEIP = DeleteHTMLToTag("<body>", lsn.MHLEIP);
                                    lsn.MHLEIP = DeleteHTMLAfterTag("</body>", lsn.MHLEIP);
                                }
                            }
                            catch
                            {
                                //Resume Next
                            }
                        }
                    }
                }



                SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public string DeleteHTMLToTag(string tag, string Source)
        {
            return Source.Remove(0, Source.IndexOf(tag) + tag.Length);
        }

        public string DeleteHTMLAfterTag(string tag, string Source)
        {
            return Source.Remove(Source.IndexOf(tag), Source.Length - Source.IndexOf(tag));
        }
    }
}
