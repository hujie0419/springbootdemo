using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using System.Linq;
using System.Collections.Generic;
using Tuhu.C.Job.UserAccountCombine.Model;
using Tuhu.C.Job.UserAccountCombine.BLL;
using Quartz;
using Tuhu.MessageQueue;
using Tuhu;


namespace Tuhu.C.Job.UserAccountCombine.Job
{
    public class DupUserMonitorJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(DupUserMonitorJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Info("DupUserMonitorJob开始执行：" + DateTime.Now.ToString());

                //获取手机号不为空 and 用户状态Isactive=true 的情况下手机号重复的用户
                var stepOne_GetNeedCombineInUserTable = UACManager.GetNeedCombineInUserTable();
                if (null == stepOne_GetNeedCombineInUserTable)
                {
                    _logger.Info("DupUserMonitorJob今天没有需要合并UserId：" + DateTime.Now.ToString());
                }
                else
                {
                    var body = new StringBuilder(10000);
                    body.Append(EmailStyle);
                    body.Append(EmailTemplate);
                    foreach(var item in stepOne_GetNeedCombineInUserTable)
                    {
                        if (item != null)
                        {
                            body.Append("<tr>");
                            body.Append($"<td>{item.UserId}</td>");
                            body.Append($"<td>{item.MobileNumber}</td>");
                            body.Append($"<td>{item.IsMobileVerify}</td>");
                            body.Append($"<td>{item.CreatedTime}</td>");
                            body.Append($"<td>{item.RegisteredDateTime}</td>");
                            body.Append($"<td>{item.LastLogonTime}</td>");
                            body.Append($"<td>{item.UpdatedTime}</td>");
                            body.Append($"<td>{item.TempChannel}</td>");
                        }
                    }

                    body.Append("</tbody></table>");

                    var message = body.ToString();
                    Tuhu.TuhuMessage.SendEmail("【Warning】出现" + stepOne_GetNeedCombineInUserTable.Count() + "个手机重复账号EOM", 
                        "zhangchen3@tuhu.cn;liuchao1@tuhu.cn",
                        message);
                }
            }
            catch (Exception ex)
            {
                _logger.Info($"DupUserMonitor：运行异常=》{ex}");
            }
        }

        #region Template

        private const string EmailStyle = @"
            <style type='text/css'>
            .tableContainer {
                width: 100%;
            }
            .tableContainer th {
                height: 32px;
                background: #66ABEF;
                color: #F3F7FA;
                border: 1px solid #559BD0;
                font-size: 10px;
                font-weight: 300;
                font-family: 'Microsoft Yahei';
                text-align: center;
            }
            .tableContainer td {
                font-size: 10px;
                padding: 8px 3px 8px 3px;
                font-family: 'Microsoft Yahei';
                text-align: center;
                border: 1px solid #eee;
            }
            .tableContainer a {
                    color: #000000;
                    text-decoration: underline;
                }
            .r1{background: #FF0000}
            .r2{background: #AA4643}
            .r3{background: #DB843D}
            .cost{ background: #00bfff}
            </style>";

        private const string EmailTemplate = @"
            <p style='font-size:16px'>以下内容为系统自动发送，请勿直接回复，谢谢。</p>
              <table class='tableContainer'>
	            <thead>
                    <tr>
                        <th>UserId</th>
                        <th>手机号</th>
                        <th>手机号是否认证</th>
                        <th>创建时间</th>
						<th>注册时间</th>
						<th>最后登录时间</th>
						<th>最后更新时间</th>
						<th>注册渠道</th>
                    </tr>
                    </thead>
                    <tbody >";
        #endregion
    }
}
