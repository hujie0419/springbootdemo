using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.Business.Crm;
using Tuhu.Provisioning.DataAccess.DAO.Discovery;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.Business.Discovery
{
    public class ShareImageBLL: ConnectionBase
    {
        //public static IEnumerable<ShareImage> SelectList(string strWhere)
        //{
        //    try
        //    {
        //        return ShareImageDAL.SelectList(ProcessConnection.OpenMarketing, strWhere);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}        
        public static string ExecuteSqlForUpdate(string sql)
        {
            try
            {
                return ShareImageDAL.ExecuteSqlForUpdate(ProcessConnection.OpenMarketing, sql).ToString();
            }
            catch (Exception ex)
            {
                return "异常：" + ex.Message + "-------堆栈信息：" + ex.StackTrace;
            }
        }

        public static List<ShareImage> SelectShareList(PagerModel pager,string strWhere)
        {
            try
            {                
                DataTable dt = ShareImageDAL.SelectShareList(ProcessConnection.OpenMarketing, pager, strWhere);
                if (dt.Rows.Count > 0)
                {
                    return DTConvertShareImage(dt);
                }
                else
                    return new List<ShareImage>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateShareImages(ShareImage model)
        {
            try
            {
                return ShareImageDAL.UpdateShareImages(ProcessConnection.OpenMarketing, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateStatus(int PKID, bool isActive)
        {
            try
            {
                return ShareImageDAL.UpdateStatus(ProcessConnection.OpenMarketing, PKID, isActive);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static  UserInfo SelectUserInfo(string userId)
        {
            var row = ShareImageDAL.GetUserInfo(ProcessConnection.OpenGungnirReadOnly, userId);

            if (row == null)
                return new UserInfo();

            string uName= !string.IsNullOrEmpty(row["u_Pref5"].ToString()) ? row["u_Pref5"].ToString() : row["u_last_name"].ToString();
            if (string.IsNullOrEmpty(uName))
            {
                string phone = row["u_mobile_number"].ToString();
                uName = phone.Replace(phone.Substring(3, 4), "****");
            }
            string defaultHead = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
            string userHead = !string.IsNullOrEmpty(row["u_Imagefile"].ToString()) ?ConfigurationManager.AppSettings["DoMain_image"] + row["u_Imagefile"].ToString():defaultHead;
            string userPhone = row["u_mobile_number"].ToString();
            return new UserInfo
            {
                UserID=userId,
                NickName=uName,
                UserHead=userHead,
                UserPhone=userPhone
            };
        }

        public static ShareImage SelectShareDetailByPKID(int PKID)
        {
            var row = ShareImageDAL.SelectShareDetailByPKID(ProcessConnection.OpenMarketing, PKID);
            if (row == null)
                return new ShareImage();
            int pkid = row.Field<int>("PKID");
            List<ImagesDetail> imageDetails = DTConvertImagesDetail(ShareImageDAL.SelectShareImages(ProcessConnection.OpenMarketing, pkid));
            string userId = row.Field<Guid>("userId").ToString();
            string content = row.Field<string>("content");
            bool isActive = row.Field<bool>("isActive");
            bool isDelete = row.Field<bool>("isDelete");
            DateTime createTime = row.Field<DateTime>("createTime");
            DateTime updateTime = row.Field<DateTime>("lastUpdateTime");
            int likeCount = row.Field<int>("likesCount");
            int commentCount = row.Field<int>("commentCount");
            int shareCount = row.Field<int>("shareCount");
            return new ShareImage
            {
                PKID = pkid,
                userId = userId,
                content = content,
                isActive = isActive,
                isDelete = isDelete,
                createTime = createTime,
                lastUpdateTime = updateTime,
                likesCount = likeCount,
                commentCount = commentCount,
                shareCount = shareCount,
                images = imageDetails,
                User=SelectUserInfo(userId)
            };
        }


        public static List<ShareImage> DTConvertShareImage(DataTable dt)
        {
            List<ShareImage> modelList = new List<ShareImage>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                for (int i = 0; i < rowsCount; i++)
                {
                    #region MyRegion
                    DataRow row = dt.Rows[i];
                    int pkid = row.Field<int>("PKID");
                    string userId = row.Field<Guid>("userId").ToString();
                    string content = row.Field<string>("content");
                    bool isActive = row.Field<bool>("isActive");
                    bool isDelete = row.Field<bool>("isDelete");
                    DateTime createTime = row.Field<DateTime>("createTime");
                    DateTime updateTime = row.Field<DateTime>("lastUpdateTime");
                    int likeCount = row.Field<int>("likesCount");
                    int commentCount = row.Field<int>("commentCount");
                    int shareCount = row.Field<int>("shareCount");
                    List<ImagesDetail> imageDetails = DTConvertImagesDetail(ShareImageDAL.SelectShareImages(ProcessConnection.OpenMarketing, pkid));
                    modelList.Add(new ShareImage()
                    {
                        PKID = pkid,
                        userId = userId,
                        content = content,
                        isActive = isActive,
                        isDelete = isDelete,
                        createTime = createTime,
                        lastUpdateTime = updateTime,
                        likesCount = likeCount,
                        commentCount = commentCount,
                        shareCount = shareCount,
                        images = imageDetails,
                        User = SelectUserInfo(userId)
                    }); 
                    #endregion
                }
            }
            return modelList;

        }

        public static List<ImagesDetail> DTConvertImagesDetail(DataTable dt)
        {
            List<ImagesDetail> modelList = new List<ImagesDetail>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                for (int i = 0; i < rowsCount; i++)
                {
                    #region MyRegion
                    DataRow row = dt.Rows[i];
                    int pkid = row.Field<int>("PKID");
                    string imageUrl = row.Field<string>("imageUrl");
                    DateTime uploadTime = row.Field<DateTime>("uploadTime");
                    bool isActive = row.Field<bool>("isActive");
                    bool isDelete = row.Field<bool>("isDelete");
                    modelList.Add(new ImagesDetail()
                    {
                        PKID = pkid,
                        imageUrl = imageUrl,
                        uploadTime = uploadTime.ToString(),
                        isActive = isActive,
                        isDelete = isDelete
                    }); 
                    #endregion
                }
            }
            return modelList;

        }


    }
}
