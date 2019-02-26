using Quartz;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class UpdateVehicleBrandPinyin:IJob
    {
        private static int MINID = 0;
        private static int ThreadCount = 1;

        /// <summary>
        /// 把车 品牌 合车型 转化为拼音 update  到数据库
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {

            var dataMap = context.JobDetail.JobDataMap;
            ThreadCount = dataMap.GetInt("ThreadCount");
            ThreadCount = dataMap.GetInt("ThreadCount");
            if (ThreadCount < 1) ThreadCount = 1;
            if (ThreadCount > 20) ThreadCount = 20;
            MINID = dataMap.GetInt("MinPKID");

            if (MINID < 0) MINID = 0;
            int pageSize = 100;

            int pkid = GetMaxPkid();
            List<Task> tasks = new List<Task>();


            Action<VehicleBrandModel> action = (p) =>
            {
                UpdateVehicleBrand(p.PKID, GetPinYin(p.Brand), GetJPinYin(p.Brand), GetPinYin(p.Vehicle),GetJPinYin(p.Vehicle));
            };

            while (pkid >= MINID)
            {
                var page = GetVehicleBrands(pkid, pageSize);
                if (!page.Any()) break;

                foreach (var item in page)
                {

                    if (ThreadCount > 1)
                    {
                        tasks.Add(Task.Factory.StartNew((obj) =>
                        {
                            action.Invoke(obj as VehicleBrandModel);
                        }, item));

                        if (tasks.Count >= ThreadCount)
                        {
                            Task.WaitAll(tasks.ToArray());
                            tasks.Clear();
                        }
                    }
                    else //如果是单线程 就直接在主线程上运行即可
                    {
                        action.Invoke(item);
                    }
                }



                pkid -= pageSize;
            }

        }

        private static Pinyin4net.Format.HanyuPinyinOutputFormat format =
            new Pinyin4net.Format.HanyuPinyinOutputFormat()
            {
                CaseType = Pinyin4net.Format.HanyuPinyinCaseType.UPPERCASE,
                ToneType = Pinyin4net.Format.HanyuPinyinToneType.WITHOUT_TONE,
                VCharType = Pinyin4net.Format.HanyuPinyinVCharType.WITH_V
            };
        public string GetPinYin(string str)
        {
            string pinyinStr = string.Empty;
            foreach (char c in str)
            {
                var arr = Pinyin4net.PinyinHelper.ToHanyuPinyinStringArray(c, format);
                if (arr != null && arr.Length > 0)
                    pinyinStr += arr[0][0] + arr[0].Substring(1).ToLower();
                else
                    pinyinStr += c;
            }
            return pinyinStr;
        }

        public string GetJPinYin(string str)
        {
            string pinyinStr = string.Empty;
            foreach (char c in str)
            {
                var arr = Pinyin4net.PinyinHelper.ToHanyuPinyinStringArray(c, format);
                if (arr != null && arr.Length > 0 && arr[0].Length > 0)
                    pinyinStr += arr[0][0];
                else
                    pinyinStr += c;
            }
            return pinyinStr;
        }

        public int GetMaxPkid()
        {
            using (var cmd =
                new SqlCommand("SELECT TOP 1 PKID FROM Gungnir..tbl_Vehicle_Type WITH(NOLOCK) ORDER BY PKID DESC"))
            {
                return (int) DbHelper.ExecuteScalar(true, cmd);
            }
        }

        public IEnumerable<VehicleBrandModel> GetVehicleBrands(int maxPkid, int pageSize)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT TOP {pageSize} PKID,Brand,Vehicle FROM Gungnir..tbl_Vehicle_Type WITH(NOLOCK) WHERE PKID<={
                            maxPkid
                        } ORDER BY PKID DESC"))
            {
                return DbHelper.ExecuteSelect<VehicleBrandModel>(true, cmd);
            }
        }


        public int UpdateVehicleBrand(long pkid, string brandPinyin, string brandJpinyin, string vehiclePinyin,string vehicleJpinyin)
        {
            using (var cmd =
                new SqlCommand(
                    "UPDATE Gungnir..tbl_Vehicle_Type WITH(ROWLOCK) SET BrandPY=@BrandPY,BrandJPY=@BrandJPY,VehiclePY=@VehiclePY,VehicleJPY=@VehicleJPY WHERE PKID=@PKID")
            )
            {
                cmd.Parameters.AddWithValue("@BrandPY", brandPinyin);
                cmd.Parameters.AddWithValue("@BrandJPY", brandJpinyin);
                cmd.Parameters.AddWithValue("@VehiclePY", vehiclePinyin);
                cmd.Parameters.AddWithValue("@VehicleJPY", vehicleJpinyin);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
    }

    public class VehicleBrandModel:Tuhu.Service.Vehicle.Model.VehicleBrand
    {
        public long PKID { get; set; }
    }
}
