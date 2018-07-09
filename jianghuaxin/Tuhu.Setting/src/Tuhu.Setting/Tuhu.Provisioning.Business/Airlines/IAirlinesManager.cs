using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public interface IAirlinesManager
    {
        List<Airlines> GetAllAirlines();
        void AddAirlines(Airlines airlines);
        void DeleteAirlines(string id);
        void UpdateAirlines(Airlines airlines);
        Airlines GetCouponCategoryByID(string id);

    }
}
