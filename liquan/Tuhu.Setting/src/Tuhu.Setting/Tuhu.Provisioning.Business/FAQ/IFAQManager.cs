using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public interface IFAQManager
    {
        List<FAQ> SelectBy(string Orderchannel, string CateOne, string CateTwo, string CateThree, string Question);
        List<FAQ> SelectAll();
        void Delete(int PKID);
        void Add(FAQ fAQ);
        void Update(FAQ fAQ);
        FAQ GetByPKID(int PKID);
        List<FAQ> TousuFaqSelectBy(string Orderchannel, string CateOne, string CateTwo, string CateThree, string Question);
        List<FAQ> TousuFaqSelectAll();
        void TousuFaqDelete(int PKID);
        void TousuFaqAdd(FAQ fAQ);
        void TousuFaqUpdate(FAQ fAQ);
        FAQ TousuFaqGetByPKID(int PKID);

        List<ActivityIntroductionModel> GetAllActivityIntroductionList(string activityName, int pageIndex, int pageSize);
        int AddOrUpActivityIntroduction(ActivityIntroductionModel activity, string Type);
        int DeleteActivityIntroductionById(int ID);
    }
}
