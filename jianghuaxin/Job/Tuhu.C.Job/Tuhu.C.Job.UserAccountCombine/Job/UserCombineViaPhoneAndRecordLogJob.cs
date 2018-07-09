using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.C.Job.UserAccountCombine.Model;
using Tuhu.C.Job.UserAccountCombine.BLL;
using Quartz;
using System.Threading.Tasks;

namespace Tuhu.C.Job.UserAccountCombine.Job
{
    [DisallowConcurrentExecution]
    public class UserCombineViaPhoneAndRecordLogJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(UserCombineAndRecordLogJob));
        private const string runtimeSwitch = "UserCombineViaPhoneAndRecordLogJob";
        void IJob.Execute(IJobExecutionContext context)
        {
            _logger.Info("UserCombineViaPhoneAndRecordLogJob开始执行：" + DateTime.Now.ToString());
            if (UACManager.CheckSwitch(runtimeSwitch))
            {
                //多线程执行模块
                //try
                //{
                //    //获取UAC_NeedCombineUserId表里IsOperateSuccess=0(即操作失败，或者还没执行)的数据
                //    var stepOne_GetNeedCombineUserIdViaPhoneList = UACManager.GetNeedCombineUserIdViaPhoneList();
                //    if (null == stepOne_GetNeedCombineUserIdViaPhoneList)
                //    {
                //        _logger.Info("UserCombineViaPhoneAndRecordLogJob没有需要合并的记录：" + DateTime.Now.ToString());
                //    }
                //    else
                //    {
                //        var task_0 = new Task(() =>
                //        {
                //            UACManager.Task_CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList,
                //            runtimeSwitch, "0");
                //            System.Threading.Thread.Sleep(1000);
                //        });
                //        var task_1 = new Task(() =>
                //        {
                //            UACManager.Task_CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList,
                //            runtimeSwitch, "1");
                //            System.Threading.Thread.Sleep(1000);
                //        });
                //        var task_2 = new Task(() =>
                //        {
                //            UACManager.Task_CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList,
                //            runtimeSwitch, "2");
                //            System.Threading.Thread.Sleep(1000);
                //        });
                //        var task_3 = new Task(() =>
                //        {
                //            UACManager.Task_CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList,
                //            runtimeSwitch, "3");
                //            System.Threading.Thread.Sleep(1000);
                //        });
                //        var task_4 = new Task(() =>
                //        {
                //            UACManager.Task_CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList,
                //            runtimeSwitch, "4");
                //            System.Threading.Thread.Sleep(1000);
                //        });
                //        var task_5 = new Task(() =>
                //        {
                //            UACManager.Task_CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList,
                //            runtimeSwitch, "5");
                //            System.Threading.Thread.Sleep(1000);
                //        });
                //        var task_6 = new Task(() =>
                //        {
                //            UACManager.Task_CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList,
                //            runtimeSwitch, "6");
                //            System.Threading.Thread.Sleep(1000);
                //        });
                //        var task_7 = new Task(() =>
                //        {
                //            UACManager.Task_CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList,
                //            runtimeSwitch, "7");
                //            System.Threading.Thread.Sleep(1000);
                //        });
                //        var task_8 = new Task(() =>
                //        {
                //            UACManager.Task_CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList,
                //            runtimeSwitch, "8");
                //            System.Threading.Thread.Sleep(1000);
                //        });
                //        var task_9 = new Task(() =>
                //        {
                //            UACManager.Task_CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList,
                //            runtimeSwitch, "9");
                //            System.Threading.Thread.Sleep(1000);
                //        });

                //        task_2.Start();
                //        task_3.Start();
                //        task_4.Start();
                //        task_5.Start();
                //        task_6.Start();
                //        task_7.Start();
                //        task_8.Start();
                //        task_9.Start();

                //        Task.WaitAll(task_2, task_3, task_4, task_5, task_6, task_7, task_8, task_9);
                //    }
                //}
                //catch (AggregateException ex)                
                //{
                //    foreach (Exception inner in ex.InnerExceptions)
                //    {
                //        _logger.Info($"UserCombineViaPhoneAndRecordLogJob：运行异常=》{inner.Message}");
                //    }
                //}
                //单线程模块
                try
                {
                    //获取UAC_NeedCombineUserId表里IsOperateSuccess=0(即操作失败，或者还没执行)的数据
                    var stepOne_GetNeedCombineUserIdViaPhoneList = UACManager.GetNeedCombineUserIdViaPhoneList();
                    if (null == stepOne_GetNeedCombineUserIdViaPhoneList)
                    {
                        _logger.Info("UserCombineViaPhoneAndRecordLogJob没有需要合并的记录：" + DateTime.Now.ToString());
                    }
                    else
                    {
                        //循环遍历，对于每条记录，
                        //如果所有表都更新成功，更新IsOperateSuccess = 1，记录所有表操作日志
                        //否者，记录报错，回滚操作，记录失败
                        //stepTwo_CombineAndRecordAction计数了需要完成\成功\失败的行数
                        if (null == stepOne_GetNeedCombineUserIdViaPhoneList || !stepOne_GetNeedCombineUserIdViaPhoneList.Any())
                        {
                            _logger.Info("UserCombineViaPhoneAndRecordLogJob所有测试数据已经跑完" + DateTime.Now.ToString());
                        }
                        else
                        {
                            var stepTwo_CombineAndRecordAction = UACManager.CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList, runtimeSwitch);
                            _logger.Info("UserCombineViaPhoneAndRecordLogJob：" + stepTwo_CombineAndRecordAction);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Info($"UserCombineViaPhoneAndRecordLogJob：运行异常=》{ex}");
                }
            }
            _logger.Info("UserCombineViaPhoneAndRecordLogJob执行结束");
        }
    }
}


////循环遍历，对于每条记录，
////如果所有表都更新成功，更新IsOperateSuccess = 1，记录所有表操作日志
////否者，记录报错，回滚操作，记录失败
////stepTwo_CombineAndRecordAction计数了需要完成\成功\失败的行数
//string onceRunLastWord = ConfigurationManager.AppSettings["UserCombineViaMobileOnceRunContainLastWord"];

//if (!string.IsNullOrWhiteSpace(onceRunLastWord) && onceRunLastWord != "")
//{
//    stepOne_GetNeedCombineUserIdViaPhoneList =
//        stepOne_GetNeedCombineUserIdViaPhoneList
//        .Where(_item => _item.MobileNumber.EndsWith(onceRunLastWord))
//        .ToList();
//}

//if (null == stepOne_GetNeedCombineUserIdViaPhoneList || !stepOne_GetNeedCombineUserIdViaPhoneList.Any())
//{
//    _logger.Info("UserCombineViaPhoneAndRecordLogJob没有手机号字段以" + onceRunLastWord + "为结尾的记录需要合并：" + DateTime.Now.ToString());
//}
//else
//{
//    var stepTwo_CombineAndRecordAction = UACManager.CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList, runtimeSwitch);
//    _logger.Info("UserCombineViaPhoneAndRecordLogJob：" + stepTwo_CombineAndRecordAction);
//}
//List<string> testList = new List<string>();
//testList.Add("15921584935");  //张晨

//stepOne_GetNeedCombineUserIdViaPhoneList = (from p in stepOne_GetNeedCombineUserIdViaPhoneList
//                                            where testList.Contains(p.MobileNumber)
//                                            select p).ToList();

//if (stepOne_GetNeedCombineUserIdViaPhoneList != null && stepOne_GetNeedCombineUserIdViaPhoneList.Any())
//    stepOne_GetNeedCombineUserIdViaPhoneList = stepOne_GetNeedCombineUserIdViaPhoneList.Take(20).ToList();
//else
//{
//    _logger.Info("UserCombineViaPhoneAndRecordLogJob所有测试数据已经跑完" + DateTime.Now.ToString());
//}

//var stepTwo_CombineAndRecordAction = UACManager.CombineAndRecordActionForPhoneCombile(stepOne_GetNeedCombineUserIdViaPhoneList, runtimeSwitch);
//_logger.Info("UserCombineViaPhoneAndRecordLogJob：" + stepTwo_CombineAndRecordAction);
