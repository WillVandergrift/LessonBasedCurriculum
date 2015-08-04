using lbcLibrary;
using acManagement;
using acManagement;
using Telerik.OpenAccess;
using System.Collections.Generic;

namespace LBC.Helper
{
    class Session
    {
        private static LbcDistrict curDistrict = new LbcDistrict();
        private static LbcDbContext dbContext = new LbcDbContext();
        private static acDbContext managementContext = new acDbContext();
        private static LbcUser curUser = new LbcUser();

        public static LbcDbContext DbContext
        {
            get
            {
                return dbContext;
            }
            set
            {
                if (dbContext != value)
                {
                    DbContext = value;
                }
            }
        }
        public static acDbContext ManagementContext
        {
            get
            {
                return managementContext;
            }
            set
            {
                if (managementContext != value)
                {
                    ManagementContext = value;
                }
            }
        }
        public static LbcDistrict CurDistrict
        {
            get
            {
                return curDistrict;
            }
            set
            {
                if (curDistrict != value)
                {
                    curDistrict = value;
                }
            }
        }
        public static LbcUser CurUser
        {
            get
            {
                return curUser;
            }
            set
            {
                if (curUser != value)
                {
                    curUser = value;
                }
            }
        }

        /// <summary>
        /// Load the district settings and connect to the district's database
        /// </summary>
        /// <param name="districtCode">The district's code</param>
        /// <returns></returns>
        public static bool LoadDistrictSettings(string districtCode)
        {
            try
            {
                managementContext = new acDbContext("data source=204.51.98.176;initial catalog=AC_Management;persist security info=True;user id=ac_admin;password=LbWv09");

                curDistrict = District.GetDistrictSettings(districtCode);
                dbContext = new LbcDbContext(curDistrict.ConnectionString);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
