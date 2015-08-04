using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lbcLibrary;
using acManagement;
using System.Collections.ObjectModel;
using LBC.Helper;

namespace LBC.Interfaces
{
    interface IExpectationModel
    {
        ObservableCollection<Expectation> LoadCourseExpectations(long? ExpectationStart, long? ExpectationStop);
        bool LoadCurriculumExpectations(ObservableCollection<Course> Curriculum, ObservableCollection<CourseMeta> MetaData);
        ObservableCollection<LbcExpectationIndex> LoadExpectationIndex();
        ObservableCollection<LbcExpectationIndex> GetExpectationIndexGrade(ref ObservableCollection<LbcExpectationIndex> expectationList, string txt_Source);
        ObservableCollection<LbcExpectationIndex> GetExpectationIndexArea(ref ObservableCollection<LbcExpectationIndex> expectationList, string txt_Source, string txt_Grade);
        ObservableCollection<LbcExpectationIndex> GetExpectationIndexCourse(ref ObservableCollection<LbcExpectationIndex> expectationList, string txt_Source, string txt_Grade, string txt_Area);
        ObservableCollection<LbcExpectationIndex> GetExpectationIndexVersion(ref ObservableCollection<LbcExpectationIndex> expectationList, string txt_Source, string txt_Grade, string txt_Area, string txt_course);
        bool UpdateCourseExpectations(LbcExpectationIndex newIndex, Course crs, ref ObservableCollection<CourseMeta> metaData);
    }
}
