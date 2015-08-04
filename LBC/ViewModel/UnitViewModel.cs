using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using lbcLibrary;
using LBC.Interfaces;
using LBC.DataModels;

namespace LBC.ViewModel
{
    class UnitViewModel
    {
        // =======================================================
        // Events
        // =======================================================
        public event PropertyChangedEventHandler PropertyChanged;

        // =======================================================
        // Private Variables
        // =======================================================
        private Unit _CurUnit;
        private IUnitModel untContext = new UnitModel();

        // =======================================================
        // Public Properties
        // =======================================================
        public Unit CurUnit
        {
            get
            {
                return _CurUnit;
            }
            set
            {
                if (value != null)
                {
                    _CurUnit = value;
                    NotifyPropertyChanged("CurUnit");
                }
            }
        }

        // =======================================================
        // Methods
        // =======================================================

        /// <summary>
        /// Create a new unit
        /// </summary>
        /// <param name="crs">The course to place the unit into</param>
        /// <returns></returns>
        public bool CreateNewUnit(Course crs)
        {
            try
            {
                Unit newUnit = new Unit();

                newUnit.ID = Guid.NewGuid();
                newUnit.CourseID = crs.ID;
                newUnit.Course = crs;
                newUnit.UnitName = "New Unit";
                newUnit.UnitNumber = (crs.Units.Count + 1);

                crs.Units.Add(newUnit);

                return true;
            }
            catch
            {
                //An error occured
                return false;
            }
        }

        /// <summary>
        /// Delete the specified lesson
        /// </summary>
        /// <param name="unt">The unit to be deleted</param>
        /// <returns></returns>
        public bool DeleteUnit(Unit unt)
        {
            try
            {
                //Remove the unit from the course's units list
                unt.Course.Units.Remove(unt);

                //Mark the unit for deletion
                return untContext.DeleteUnit(unt);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Move a unit into a course
        /// </summary>
        /// <param name="unt">The unit to move</param>
        /// <param name="newCrs">The course that the unit is going into</param>
        /// <returns></returns>
        public bool MoveUnit(Unit unt, Course newCrs)
        {
            try
            {
                unt.CourseID = newCrs.ID;
                unt.Course = newCrs;

                //Remove the unit from its current course
                unt.Course.Units.Remove(unt);

                //Move the unit into its new course
                newCrs.Units.Add(unt);

                return true;
            }
            catch
            {
                //something went wrong
                return false;
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
