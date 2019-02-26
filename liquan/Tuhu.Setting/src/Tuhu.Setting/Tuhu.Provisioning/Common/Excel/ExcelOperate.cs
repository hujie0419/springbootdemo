using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Tuhu.Provisioning.Common.Excel;

namespace Tuhu.Provisioning.Common
{
    public class ExcelOperate
    {
        /// <summary>
        /// DataTable导出数据为Excel类型
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="fields"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static HttpResponseMessage DtToExcel(DataTable dtSource, IDictionary fields, string fileName)
        {
            if (dtSource != null && dtSource.Rows != null && dtSource.Rows.Count == 0)
            {
                dtSource.Rows.Add(dtSource.NewRow());
            }
            MemoryStream ms = (new ExcelFun()
            {
                HtFields = fields
            }.DtToExcel(dtSource)) as MemoryStream;

            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(ms) };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-excel");

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlEncode(fileName)
            };
            return response;
        }
    }
}