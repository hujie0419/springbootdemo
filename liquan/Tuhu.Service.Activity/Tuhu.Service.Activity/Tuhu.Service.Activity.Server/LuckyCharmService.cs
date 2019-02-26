using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{

    public class LuckyCharmService : ILuckyCharmService
    {
        public LuckyCharmService()
        {
            AutoMappers.Configuration.Configure();
        }
        public async Task<OperationResult<bool>> AddLuckyCharmActivityAsync(AddLuckyCharmActivityRequest request)
        {
            return await LuckyCharmManager.AddActivityAsync(request);
        }
        public async Task<OperationResult<LuckyCharmActivityInfoResponse>> GetLuckyCharmActivityAsync(int pkid)
        {
            return await LuckyCharmManager.GetActivityAsync(pkid);
        }
        public async Task<OperationResult<bool>> AddLuckyCharmUserAsync(AddLuckyCharmUserRequest request)
        {
            return await LuckyCharmManager.AddActivityUserAsync(request);
        }

        public async Task<OperationResult<bool>> UpdateLuckyCharmUserAsync(UpdateLuckyCharmUserRequest request)
        {
            return await LuckyCharmManager.UpdateActivityUserAsync(request);
        }

        public async Task<OperationResult<PageLuckyCharmActivityResponse>> PageLuckyCharmActivityAsync(PageLuckyCharmActivityRequest request)
        {
            return await LuckyCharmManager.PageActivityActivityAsync(request);
        }

        public async Task<OperationResult<PageLuckyCharmUserResponse>> PageLuckyCharmUserAsync(PageLuckyCharmUserRequest request)
        {
            return await LuckyCharmManager.PageActivityUserAsync(request);
        }

        public async Task<OperationResult<bool>> AuditLuckyCharmUserAsync(int pkid)
        {
            return await LuckyCharmManager.AuditActivityStatusAsync(pkid);
        }

        public async Task<OperationResult<bool>> DelLuckyCharmUserAsync(int pkid)
        {
            return await LuckyCharmManager.DelUserAsync(pkid);
        }


        public async Task<OperationResult<bool>> DelLuckyCharmActivityAsync(int pkid)
        {
            return await LuckyCharmManager.DelActivityAsync(pkid);
        }


    }
}
