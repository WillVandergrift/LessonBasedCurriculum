using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBC.Interfaces;
using LBC.Helper;

namespace LBC.DataModels
{
    class UnitModel : IUnitModel
    {

        /// <summary>
        /// Delete the specified Unit
        /// </summary>
        /// <param name="unt">The unit to be deleted</param>
        public bool DeleteUnit(lbcLibrary.Unit unt)
        {
            try
            {
                Session.DbContext.Delete(unt);

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
