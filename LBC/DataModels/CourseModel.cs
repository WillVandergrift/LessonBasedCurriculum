using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBC.Interfaces;
using LBC.Helper;
using lbcLibrary;
using System.Collections.ObjectModel;

namespace LBC.DataModels
{
    class CourseModel : ICourseModel
    {
        /// <summary>
        /// Create a new course and add it to the current user's course list
        /// </summary>
        /// <returns></returns>
        public Course CreateCourse(ref ObservableCollection<CourseMeta> crsMeta)
        {
            try
            {
                //Create the new Course
                Course newCourse = new Course();
                newCourse.Name = "New Course";
                newCourse.ID = Guid.NewGuid();

                //Add the new course to the current user's course list
                CoursePermission newCoursePermissions = new CoursePermission();
                newCoursePermissions.Course = newCourse;
                newCoursePermissions.CourseID = newCourse.ID;
                newCoursePermissions.LbcUser = Session.CurUser;
                newCoursePermissions.UserID = Session.CurUser.ID;
                newCoursePermissions.Permission = "Lead Writer";
                newCourse.CoursePermissions.Add(newCoursePermissions);

                //Create the course meta data
                CourseMeta newCourseMeta = new CourseMeta();
                newCourseMeta.CourseID = newCourse.ID;
                newCourseMeta.Permission = "Lead Writer";
                crsMeta.Add(newCourseMeta);

                //Add the course to the database context and the user's course list that's loaded into the program
                Session.DbContext.Add(newCourse);
                return newCourse;
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Remove the selected course from the database
        /// </summary>
        /// <param name="crs"></param>
        /// <returns></returns>
        public bool DeleteCourse(Course crs)
        {
            try
            {
                Session.DbContext.Delete(crs);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
