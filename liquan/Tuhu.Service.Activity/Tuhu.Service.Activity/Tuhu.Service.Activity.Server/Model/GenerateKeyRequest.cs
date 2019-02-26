using System.Linq;

namespace Tuhu.Service.Activity.Server.Model
{
    public class GenerateFlashSaleKey
    {
        public string PersonalkeyUserId;
        public string PersonalkeyDeviceId;
        public string PersonalkeyUseTel;
        public string PlacekeyUserId;
        public string PlacekeyDeviceId;
        public string PlacekeyUseTel;
        public GenerateFlashSaleKey(GenerateKeyRequest request)
        {
            PersonalkeyUserId = GenerateKey(
                                request.UserId,
                                request.UserId,
                                "userId",
                                request.ActivityId,
                                request.Pid,
                                ((int)LimitType.PersonalLimit).ToString());
            PersonalkeyDeviceId = GenerateKey(request.UserId,
                                request.DeviceId,
                                "deviceId",
                                new[]
                                {request.ActivityId.ToString(),
                                request.Pid,
                                ((int)LimitType.PersonalLimit).ToString()
                                });
            PersonalkeyUseTel = GenerateKey(
                                    request.UserId.ToString(),
                                    request.UserTel?.ToString(),
                                    "userTel",
                                    new[]
                                    {request.ActivityId.ToString(),
                                                    request.Pid,
                                                    ((int)LimitType.PersonalLimit).ToString()
                                     });
            if (!request.IsAllPlaceLimit)
            {
                PlacekeyUserId = GenerateKey(request.UserId.ToString(),
                    request.UserId.ToString(),
                    "userId",
                    new[]
                    {
                        request.ActivityId.ToString(),
                        ((int) LimitType.PlaceLimit).ToString()
                    });
                PlacekeyDeviceId = GenerateKey(request.UserId.ToString(),
                    request.DeviceId?.ToString(),
                    "deviceId",
                    new[]
                    {
                        request.ActivityId.ToString(),
                        ((int) LimitType.PlaceLimit).ToString()
                    });
                PlacekeyUseTel = GenerateKey(request.UserId.ToString(),
                    request.UserTel?.ToString(),
                    "useTel",
                    new[]
                    {
                        request.ActivityId.ToString(),
                        ((int) LimitType.PlaceLimit).ToString()
                    });
            }
            else
            {
                PlacekeyUserId = GenerateKey(request.UserId.ToString(),
    request.UserId.ToString(),
    "userId",
    new[]
    {
                        request.ActivityId.ToString(),
                        ((int) LimitType.AllPlaceLimit).ToString()
    });
                PlacekeyDeviceId = GenerateKey(request.UserId.ToString(),
                    request.DeviceId?.ToString(),
                    "deviceId",
                    new[]
                    {
                        request.ActivityId.ToString(),
                        ((int) LimitType.AllPlaceLimit).ToString()
                    });
                PlacekeyUseTel = GenerateKey(request.UserId.ToString(),
                    request.UserTel?.ToString(),
                    "useTel",
                    new[]
                    {
                        request.ActivityId.ToString(),
                        ((int) LimitType.AllPlaceLimit).ToString()
                    });
            }
        }
        public static string GenerateKey(string userid, string firstKey, string firstkeyprefix, params string[] list)
        {
            if (list == null || !list.Any())
                return null;
            return string.Concat("prefixnew4",
                (string.IsNullOrEmpty(firstKey) ? (userid + firstkeyprefix + "/") : (firstKey + "/")),
                string.Join("/", list.Select(r => r)));
        }
    }

    public class GenerateKeyRequest
    {
        public string UserId { get; set; }
        public string DeviceId { get; set; }
        public string UserTel { get; set; }
        public string ActivityId { get; set; }
        public string Pid { get; set; }
        public bool IsAllPlaceLimit { get; set; }
    }
}
