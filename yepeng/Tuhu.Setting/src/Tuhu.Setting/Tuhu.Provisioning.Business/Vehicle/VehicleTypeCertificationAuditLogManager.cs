using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Models;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Vehicle;
using Tuhu.Service.Vehicle.Request;

namespace Tuhu.Provisioning.Business.Vehicle
{
    public class VehicleTypeCertificationAuditLogManager
    {
        private static readonly ILog logger = LoggerFactory.GetLogger("VehicleTypeCertificationAuditLogManager");


        public bool InsertLog(VehicleTypeCertificationAuditLogModel model)
        {
            return DalVehicleTypeCertificationAuditLog.InsertLog(model);
        }

        public List<VehicleTypeCertificationAuditLogModel> SelectLogs(List<Guid> carIds)
        {
            return DalVehicleTypeCertificationAuditLog.SelectLogs(carIds);
        }

        public List<VehicleTypeCertificationAuditLogModel> SelectLogsByCarId(Guid carId)
        {
            return DalVehicleTypeCertificationAuditLog.SelectLogsByCarId(carId);
        }

        public ListModel<VehicleAuditInfoModel> SelectVehicleAuditInfo(VehicleAuthAuditRequest request)
        {
            return DalVehicleTypeCertificationAuditLog.SelectVehicleAuditInfo(request);
        }

        public bool UpdateVehicleAuditStatus(Guid carId,int status)
        {
            VehicleAuditInfoModel appeal = DalVehicleTypeCertificationAuditLog.GetVehicleAuthAppeal(carId);
            using (var client = new VehicleClient())
            {
                var response = client.UpdateVehicleTypeCertificationStatus(new VehicleTypeCertificationRequest()
                {
                    CarId = carId,
                    Status = status,
                    CarNo = appeal?.CarNumber,
                    Channel = appeal?.Channel,
                    EngineNo = appeal?.EngineNo,
                    Vehicle_license_img = appeal?.ImageUrl,
                    User_IdCard_img = appeal?.IdCardUrl,
                    VinCode = appeal?.VinCode,
                    Registrationtime = appeal?.car_Registrationtime
                });
                if (response.Success && status > 0)
                {
                    using (var db = Tuhu.Component.Common.DbHelper.CreateDefaultDbHelper())
                    {
                        DalVehicleTypeCertificationAuditLog.UpdateVehicleAuditStatus(db, carId, status);
                    }
                }
                return response.Success;
            }
        }
    }
}
