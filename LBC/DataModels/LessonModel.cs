using lbcLibrary;
using LBC.Interfaces;
using LBC.Helper;

namespace LBC.DataModels
{
    class LessonModel : ILessonModel
    {
        /// <summary>
        /// Delete the specified lesson
        /// </summary>
        /// <param name="lsn">The lesson to delete</param>
        /// <returns></returns>
        public bool DeleteLesson(Lesson lsn)
        {
            try
            {
                Session.DbContext.Delete(lsn);

                return true;
            }
            catch
            {
                //Something went wrong
                return false;
            }
            
        }
    }
}
