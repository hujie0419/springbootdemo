using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.MMS.Web.Domain.UserFilter;

namespace Tuhu.MMS.Web.Service
{
    public partial class UserFilterService
    {


        public static async Task<IEnumerable<UserFilterRuleJobDetail>> SelectUserFilterRuleJobDetailsAsync(int jobid)
        {
            var result = await Repository.UserFilterRule.SelectUserFilterRuleJobDetailsAsync(jobid);
            return result;
        }

        public static async Task<IEnumerable<UserFilterValueConfig>> SelectAllUserFilterValueConfigAsync()
        {
            //var result = await Repository.UserFilterRule.SelectAllUserFilterValueConfigAsync();
            var result = await TuhuMemoryCache.Instance.GetOrSetAsync("AllUserFilterValueConfig", async () => await Repository.UserFilterRule.SelectAllUserFilterValueConfigAsync(), TimeSpan.FromDays(1));
            return result;
        }

        public static async Task<IEnumerable<UserFilterRuleJob>> SelectUserFilterRuleJobsAsync()
        {
            var result = await Repository.UserFilterRule.SelectUserFilterRuleJobsAsync();
            return result;
        }

        public static async Task<bool> SaveJobDescriptionAsync(int jobid, string description)
        {
            var result = await Repository.UserFilterRule.SaveJobDescriptionAsync(jobid, description);
            return result;
        }

        public static async Task<bool> SaveJobRunStatusAsync(int jobid, JobStatus status)
        {
            var result = true;
            if (status == JobStatus.WaittingForRun)
            {
                result = await SetFilterJobRunAsync(jobid);
            }
            if (result)
            {
                await Repository.UserFilterRule.SaveJobRunStatusAsync(jobid, status);
            }
            return result;
        }

        public static async Task<UserFilterRuleJob> SelectUserFilterRuleJobsAsync(int jobid)
        {
            var result = await Repository.UserFilterRule.SelectUserFilterRuleJobsAsync(jobid);
            return result;
        }
        public static async Task<int> InsertUserFilterRuleJobAsync(UserFilterRuleJob job)
        {
            var result = await Repository.UserFilterRule.InsertUserFilterRuleJobAsync(job);
            return result;
        }

        public static async Task<IEnumerable<UserFilterValueConfig>> SelectUserFilterValueConfigByParentValueAsync(string parentvalue)
        {
            //var result = await Repository.UserFilterRule.SelectUserFilterValueConfigByParentValueAsync(parentvalue);
            //return result;
            var result = (await SelectAllUserFilterValueConfigAsync()).Where(x => x.ParentValue == parentvalue);
            return result;
        }

        public static async Task<IEnumerable<UserFilterValueConfig>> SelectUserFilterValueConfigByColNameAsync(string colname)
        {
            //var result = await Repository.UserFilterRule.SelectUserFilterValueConfigByColNameAsync(colname);
            //return result;
            var result = (await SelectAllUserFilterValueConfigAsync()).Where(x => x.ColName == colname);
            return result;
        }
        public static async Task<int> DeleteFilterRuleJobDetailAsync(string batchid)
        {
            var result = await Repository.UserFilterRule.DeleteFilterRuleJobDetailAsync(batchid);
            return result;
        }

        public static string SelectColsByTableName(string tablename)
        {
            string dm_userintegraldetail = $"userid,inintegral,outintegral,loaddate";

            string dm_userlog =
                $"deviceid,channel,typename,businesslines,category1,category2,category3,specifications,pid,pv,loaddate";
            string dm_userorderdetail =
                $"userid,typename,orderchannel,channelgather,channeltype,pid,category,businesslines,category1,category2,category3,installtype,orderdescription,ordernum,num,salesamount1,salesamount2,cost,loaddate";
            string dm_userdetail =
                $"userid,deviceid,usertel,username,registrationtime,appbindingtime,usergrade,growthvalue,gender,province,city,vehicleid,tid,vehiclebrand,vehiclename,vehiclepailiang,vehiclenian,vehiclesalesname,fiveavgprice,twoavgprice,defaultspecifications,kilometer,onroaddtae,currentintegral,loaddate";
            switch (tablename)
            {
                case "dm_userintegraldetail":
                    return dm_userintegraldetail;
                case "dm_userlog":
                    return dm_userlog;
                case "dm_userorderdetail":
                    return dm_userorderdetail;
                case "dm_userdetail":
                    return dm_userdetail;
                default:
                    throw new NotSupportedException();
            }
        }
        public static async Task<bool> SetFilterJobRunAsync(int jobid)
        {
            var job = await SelectUserFilterRuleJobsAsync(jobid);
            var details = await SelectUserFilterRuleJobDetailsAsync(jobid);
            var filterresults = await SelectUserFilterResultConfigAsync(jobid);
            var resulttables = filterresults.Select(x => x.TableName).Distinct().ToList();
            List<string> addgroups = new List<string>();
            List<string> orgroups = new List<string>();
            List<string> exceptgroups = new List<string>();
            List<string> addtemplatetables = new List<string>();
            List<string> ortemplatetables = new List<string>();
            List<string> excepttemplatetables = new List<string>();
            List<string> finallsqls = new List<string>();
            List<string> outputresulttables = new List<string>();

            if (details != null && details.Any())
            {
                details = details.Where(x =>
                    !string.IsNullOrEmpty(x.SearchKey) && !string.IsNullOrEmpty(x.SearchValue));
                var groupresults = details.GroupBy(x => new
                {
                    tablename = x.TableName,
                    jointype = x.JoinType
                }).Select(x => new
                {
                    key = x.Key,
                    jobs = x
                });



                Func<CompareType, string> GetCompareType = comparetpye =>
                {
                    switch (comparetpye)
                    {
                        case CompareType.Equal:
                            return " = ";
                        case CompareType.Greater:
                            return " > ";
                        case CompareType.GreaterOrEqual:
                            return " >= ";
                        case CompareType.Less:
                            return " < ";
                        case CompareType.LessOrEqual:
                            return " <= ";
                        case CompareType.DateFromToday:
                            return " >= ";
                        default:
                            throw new NotSupportedException();
                    }
                };
                foreach (var groupresult in groupresults)
                {
                    var table = groupresult.key.tablename;
                    var jointype = groupresult.key.jointype;
                    string basesql = "";
                    string temporarytablename = GetTableTemporaryTableName(table, jointype);
                    if (table == "dm_userdetail")
                    {
                        basesql = $@"CREATE TEMPORARY TABLE {temporarytablename} as select b.userid,b.deviceid   
                                from dm_userdetail as t join dm_userlogin as b 
                                    on t.userid=b.userid and t.deviceid=b.deviceid ";
                    }
                    else if (table == "dm_userlog")
                    {
                        basesql = $@"CREATE TEMPORARY TABLE {temporarytablename} as  select b.userid,b.deviceid   
                                from dm_userlog as t join dm_userlogin as b 
                                    on  t.deviceid=b.deviceid ";
                    }
                    else if (table == "dm_userorderdetail")
                    {
                        basesql = $@"CREATE TEMPORARY TABLE {temporarytablename} as select b.userid,b.deviceid   
                                from dm_userorderdetail as t join dm_userlogin as b 
                                    on  t.userid=b.userid ";
                    }
                    var wheretemp = groupresult.jobs.Where(x => x.CompareType != CompareType.DateFromToday)
                        .Select(x => $" t.{x.SearchKey} {GetCompareType(x.CompareType)} '{x.SearchValue}' ").ToList();
                    if (groupresult.jobs.Any(x => x.CompareType == CompareType.DateFromToday))
                    {
                        var temps = groupresult.jobs.Where(x => x.CompareType == CompareType.DateFromToday);
                        if (temps != null && temps.Any())
                        {
                            foreach (var temp in temps)
                            {
                                wheretemp.Add($" t.{temp.SearchKey} > '{DateTime.Now.AddDays(-1 * Convert.ToInt32(temp.SearchValue)).Date}' ");
                                wheretemp.Add($" t.{temp.SearchKey} < '{DateTime.Now.Date}' ");
                            }
                        }
                    }
                    if (jointype == JoinType.And)
                    {
                        addtemplatetables.Add(temporarytablename);
                        string wherequery = string.Join(" and ", wheretemp);
                        addgroups.Add(basesql + " where " + wherequery);
                    }
                    if (jointype == JoinType.Or)
                    {
                        ortemplatetables.Add(temporarytablename);
                        string wherequery = string.Join(" or ", wheretemp);
                        orgroups.Add(basesql + " where " + wherequery);
                    }
                    if (jointype == JoinType.Except)
                    {
                        excepttemplatetables.Add(temporarytablename);
                        string wherequery = string.Join(" and ", wheretemp);
                        exceptgroups.Add(basesql + " where " + wherequery);
                    }
                }
                finallsqls.AddRange(addgroups);
                finallsqls.AddRange(orgroups);
                finallsqls.AddRange(exceptgroups);

                if (addtemplatetables.Any())
                {
                    string temp = string.Join(" union all ", addtemplatetables.Select(x => $" select userid,deviceid from {x} "));
                    temp = $"CREATE TEMPORARY TABLE addresults as {temp}  ";
                    finallsqls.Add(temp);
                }
                if (ortemplatetables.Any())
                {
                    string temp = string.Join(" union all ", ortemplatetables.Select(x => $" select userid,deviceid from {x} "));
                    temp = $"CREATE TEMPORARY TABLE orresults as {temp}  ";
                    finallsqls.Add(temp);
                }
                if (excepttemplatetables.Any())
                {
                    string temp = string.Join(" union all ", excepttemplatetables.Select(x => $" select userid,deviceid from {x} "));
                    temp = $"CREATE TEMPORARY TABLE exceptresults as  {temp}  ";
                    finallsqls.Add(temp);
                }
                if (addtemplatetables.Any() && ortemplatetables.Any())
                {
                    string temp = $"CREATE TEMPORARY TABLE inresults as  select * from addresults union all select * from orresults  ";
                    finallsqls.Add(temp);
                }
                else if (addtemplatetables.Any() && !ortemplatetables.Any())
                {
                    string temp = $"CREATE TEMPORARY TABLE inresults as  select * from addresults ";
                    finallsqls.Add(temp);
                }
                else if (!addtemplatetables.Any() && ortemplatetables.Any())
                {
                    string temp = $"CREATE TEMPORARY TABLE inresults as  select * from orresults  ";
                    finallsqls.Add(temp);
                }
                if (addtemplatetables.Any() || ortemplatetables.Any())
                {
                    if (excepttemplatetables.Any())
                    {
                        string temp =
                            "CREATE TEMPORARY TABLE finallresults_userid as select   o.* from inresults as o WHERE  o.userid not in (select userid from exceptresults) ";
                        finallsqls.Add(temp);
                        temp =
                            "CREATE TEMPORARY TABLE finallresults as select   o.* from finallresults_userid as o WHERE  o.deviceid not in (select deviceid from exceptresults) ";
                        finallsqls.Add(temp);
                        //string temp =
                        //    "CREATE TEMPORARY TABLE finallresults as select   o.* from inresults as o  left join exceptresults as e on  e.deviceid = o.deviceid and e.userid = o.userid where o.deviceid is  null and  o.userid is null  ";
                        //finallsqls.Add(temp);

                    }
                    else
                    {
                        string temp =
                            "CREATE TEMPORARY TABLE finallresults as select  o.* from inresults as o  ";
                        finallsqls.Add(temp);
                    }
                }
                var s = string.Join(" ; ", finallsqls);
            }
            List<string> resultsqls = new List<string>();

            if (filterresults != null && filterresults.Any())
            {
                filterresults.Select(x => x.ColName).OrderByDescending(x => x);
                var temps = filterresults.GroupBy(x => x.TableName).Select(x => new
                {
                    table = x.Key,
                    cols = x.Select(s => s.ColName)
                });
                foreach (var temp in temps)
                {
                    string table = temp.table;
                    string cols = SelectColsByTableName(table);
                    cols = string.Join(",", cols.Split(',').Select(x => $"t.{x}"));

                    outputresulttables.Add(table + "_" + jobid);
                    string sql = $" CREATE  TABLE {table + "_" + jobid} as select {cols},{jobid} as jobid from {temp.table} as t  ";
                    if (table == "dm_userdetail")
                    {
                        //if (addtemplatetables.Any())
                        //{
                        //    sql += $" and userid in (select userid from addresults ) ";
                        //}
                        //if (ortemplatetables.Any())
                        //{
                        //    sql += $" or (userid in (select userid from orresults ) )";
                        //}
                        //if (addtemplatetables.Any() || ortemplatetables.Any())
                        //{
                        //    string sqltemp = sql + " and userid in (select userid from inresults)  ";
                        //    resultsqls.Add(sql);
                        //}
                        //if (exceptgroups.Any())
                        //{
                        //    sql += $" and userid not in (select userid from exceptresults ) ";
                        //}
                        sql += " where t.userid in (select f.userid from finallresults as f) ";
                    }
                    else if (table == "dm_userlog")
                    {
                        //if (addtemplatetables.Any())
                        //{
                        //    sql += $" and deviceid in (select deviceid from addresults ) ";
                        //}
                        //if (ortemplatetables.Any())
                        //{
                        //    sql += $" or (deviceid in (select deviceid from orresults ) )";
                        //}
                        //if (exceptgroups.Any())
                        //{
                        //    sql += $" and deviceid not in (select deviceid from exceptresults ) ";
                        //}
                        sql += " where t.deviceid in (select f.deviceid from finallresults as f) ";
                    }
                    else if (table == "dm_userorderdetail")
                    {
                        //if (addtemplatetables.Any())
                        //{
                        //    sql += $" and userid in (select userid from addresults ) ";
                        //}
                        //if (ortemplatetables.Any())
                        //{
                        //    sql += $" or (userid in (select userid from orresults ) ) ";
                        //}
                        //if (exceptgroups.Any())
                        //{
                        //    sql += $" and userid not in (select userid from exceptresults ) ";
                        //}
                        sql += " where t.userid in (select f.userid from finallresults as f) ";
                    }
                    resultsqls.Add(sql);
                }
            }
            var s1 = string.Join(" ; ", resultsqls);
            if (finallsqls.Any() && resultsqls.Any())
            {
                return await Repository.UserFilterRule.SaveJobRunSqlsAsync(jobid, string.Join(";", finallsqls),
                    string.Join(";", resultsqls), string.Join(";", outputresulttables));
            }
            return false;
        }

        private static string GetTableTemporaryTableName(string tablename, JoinType jointype)
        {
            tablename = tablename.Replace("dm_", "").Replace("tuhubi_dm.", "");
            return (jointype + "_" + tablename).ToLower().Trim();
        }

        public static async Task<int> InsertUserFilterResultConfigAsync(UserFilterResultConfig config)
        {
            return await Repository.UserFilterRule.InsertUserFilterResultConfigAsync(config);
        }
        public static async Task<int> InsertOrUpdateUserFilterResultConfigAsync(UserFilterResultConfig config)
        {
            return await Repository.UserFilterRule.InsertOrUpdateUserFilterResultConfigAsync(config);
        }

        public static async Task<int> SelectUserFilterResultConfigCountAsync(int jobid)
        {
            return await Repository.UserFilterRule.SelectUserFilterResultConfigCountAsync(jobid);
        }
        public static async Task<IEnumerable<UserFilterResultConfig>> SelectUserFilterResultConfigAsync(int jobid)
        {
            return await Repository.UserFilterRule.SelectUserFilterResultConfigAsync(jobid);
        }

        public static async Task<bool> InsertUserFilterRuleDetailFromWebAsync(string firstCategory,
            string secondCategory, JoinType jointype, string tableName, NameValueCollection formdata, int jobid)
        {
            var basedetail = new UserFilterRuleJobDetail()
            {
                TableName = tableName,
                BasicAttribute = firstCategory,
                SecondAttribute = secondCategory,
                JoinType = jointype,
                JobId = jobid,
                BatchID = Guid.NewGuid().ToString()
            };
            if (formdata != null && formdata.AllKeys.Any())
            {
                List<string> skipkeys = new List<string>();
                //选日期时间或据今日
                if (formdata.AllKeys.Any(x => x.EndsWith("_selecttype")))
                {
                    string key = formdata.AllKeys.First(x => x.EndsWith("_selecttype"));
                    string selecttype = formdata[key];
                    string colname = key.Replace("_selecttype", "");

                    skipkeys.Add(key);
                    if (string.Equals(selecttype, "date"))
                    {
                        skipkeys.Add(colname + "_from_date");
                    }
                    else
                    {
                        skipkeys.Add(colname + "_start_date");
                        skipkeys.Add(colname + "_end_date");
                    }
                }
                //订单统计字段
                if (formdata.AllKeys.Any(x => string.Equals("countcoltype", x)))
                {
                    skipkeys.Add("countcoltype");
                    skipkeys.Add("countcoltype_start");
                    skipkeys.Add("countcoltype_end");
                    var detail = basedetail.DeepCopy();

                    detail.SearchKey = formdata["countcoltype"];
                    detail.SearchValue = formdata["countcoltype_start"];
                    detail.CompareType = CompareType.GreaterOrEqual;

                    await Repository.UserFilterRule.InsertUserFilterRuleJobDetailAsync(detail);

                    detail.SearchValue = formdata["countcoltype_end"];
                    detail.CompareType = CompareType.LessOrEqual;
                    await Repository.UserFilterRule.InsertUserFilterRuleJobDetailAsync(detail);
                }
                if (formdata.AllKeys.Any(x => x.EndsWith("_start_time")) || formdata.AllKeys.Any(x => x.EndsWith("_end_time")))
                {
                    skipkeys.AddRange(formdata.AllKeys.Where(x => x.EndsWith("_start_time")));
                    skipkeys.AddRange(formdata.AllKeys.Where(x => x.EndsWith("_end_time")));
                }
                foreach (var datakey in formdata.AllKeys)
                {
                    if (skipkeys.Contains(datakey))
                    {
                        continue;
                    }
                    var datevalue = formdata[datakey];
                    var detail = basedetail.DeepCopy();

                    if (datakey.EndsWith("start_date") || datakey.EndsWith("end_date"))
                    {
                        detail.CompareType = datakey.EndsWith("start_date")
                            ? CompareType.GreaterOrEqual
                            : CompareType.LessOrEqual;
                        var colname = datakey.Replace("_start_date", "").Replace("_end_date", "");
                        if (formdata.AllKeys.Any(x => x == colname + "_start_time"))
                        {
                            skipkeys.Add(colname + "_start_time");
                            datevalue = datevalue + " " + formdata[colname + "_start_time"];
                        }
                        else if (formdata.AllKeys.Any(x => x == colname + "_end_time"))
                        {
                            skipkeys.Add(colname + "_end_time");
                            datevalue = datevalue + " " + formdata[colname + "_end_time"];
                        }
                        detail.SearchKey = colname;
                        detail.SearchValue = datevalue;
                    }
                    else if (datakey.EndsWith("_from_date"))
                    {
                        var colname = datakey.Replace("_from_date", "");
                        detail.SearchKey = colname;
                        detail.SearchValue = datevalue;
                        detail.CompareType = CompareType.DateFromToday;
                    }
                    else if (datakey.EndsWith("_start") || datakey.EndsWith("_end"))
                    {
                        detail.CompareType = datakey.EndsWith("_start")
                            ? CompareType.GreaterOrEqual
                            : CompareType.LessOrEqual;
                        detail.SearchKey = datakey.Replace("_start", "").Replace("_end", "");
                        detail.SearchValue = formdata[datakey];
                    }
                    else
                    {
                        detail.SearchKey = datakey;
                        detail.SearchValue = formdata[datakey];
                        detail.CompareType = CompareType.Equal;
                    }
                    var result = await Repository.UserFilterRule.InsertUserFilterRuleJobDetailAsync(detail);
                }
            }
            return true;
        }
    }
}
