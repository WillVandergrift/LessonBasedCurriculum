using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using acManagement;

namespace LBC.Helper
{
    class District
    {
        public static LbcDistrict GetDistrictSettings(string districtCode)
        {
            try
            {
                IQueryable<LbcDistrict> matchingDistricts = Session.ManagementContext.LbcDistricts.Where(d => d.DistrictCode == districtCode);

                if (matchingDistricts.Count() > 0)
                {
                    if (matchingDistricts.First().IsActive == true)
                    {
                        return matchingDistricts.First();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
