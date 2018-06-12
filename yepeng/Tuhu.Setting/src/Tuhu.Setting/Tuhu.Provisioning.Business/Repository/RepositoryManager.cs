using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.Repository;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public enum ConnectionStrings
    {
        [EnumDescription("Configuration")]
        Configuration,

        [EnumDescription("Configuration_AlwaysOnRead")]
        Configuration_AlwaysOnRead,

        [EnumDescription("Tuhu_Activity")]
        Tuhu_Activity,

        [EnumDescription("Tuhu_Activity_AlwaysOnRead")]
        Tuhu_Activity_AlwaysOnRead
    }


   public class RepositoryManager
    {
        private string connectionString ;
        public RepositoryManager()
        {
           
        }

        public RepositoryManager(ConnectionStrings name)
        {
            connectionString = EnumStringHelper.GetEnumDescription(name);
        }
        private string GetConnectionString(bool isReadOnly=false)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                if (!isReadOnly)
                {
                    return "Configuration";
                }
                else
                    return "Configuration_AlwaysOnRead";
            }
            else
                return connectionString;
        }


        public void Add<T>(T model) where T : class
        {
            using (var db = new RepositoryBase(GetConnectionString()))
            {
                db.Insert<T>(model);
            }
        }

        public void Add<T>(List<T> entities)where T:class
        {
            using (var db = new RepositoryBase(GetConnectionString()))
            {
                db.Insert<T>(entities);
            }
        }

        public void Update<T>(T model) where T : class
        {
            using (var db = new RepositoryBase(GetConnectionString()))
            {
                db.Update<T>(model);
            }
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            using (var db = new RepositoryBase(GetConnectionString()))
            {
                db.Delete<T>(predicate);
            }
        }


        public IEnumerable<T> GetEntityList<T>(string strSQL) where T:class
        {
            using (var db = new RepositoryBase(GetConnectionString(true)))
            {
               return db.FindList<T>(strSQL);
            }
        }

        public IEnumerable<T> GetEntityList<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            using (var db = new RepositoryBase(GetConnectionString(true)))
            {
                return db.FindList<T>(predicate);
            }
            return null;
        }


        public List<T> GetEntityList<T>(Expression<Func<T, bool>> predicate, Pagination pagination) where T : class,new ()
        {
            using (var db = new RepositoryBase(GetConnectionString(true)))
            {
                return db.FindList<T>(predicate, pagination);
            }
        }

        public List<T> GetEntityList<T>(string strSQL,  System.Data.SqlClient.SqlParameter[] parameters)where T:class
        {
            using (var db = new RepositoryBase(GetConnectionString(true)))
            {
                return db.FindList<T>(strSQL, parameters);
            }
        }

        public List<T> GetEntityList<T>(Pagination pagination) where T:class,new ()
        {
            using (var db = new RepositoryBase(GetConnectionString(true)))
            {
                return db.FindList<T>(pagination);
            }
        }


        public T GetEntity<T>(Expression<Func<T, bool>> predicate)where T:class
        {
            using (var db = new RepositoryBase(GetConnectionString(true)))
            {
                return db.FindEntity<T>(predicate);
            }
        }

        public T GetEntity<T>(object keyValue) where T : class
        {
            using (var db = new RepositoryBase(GetConnectionString(true)))
            {
                return db.FindEntity<T>(keyValue);
            }
        }

        public RepositoryBase BeginTrans()
        {
            return new RepositoryBase(GetConnectionString())?.BeginTrans();
        }

     


    }




}
