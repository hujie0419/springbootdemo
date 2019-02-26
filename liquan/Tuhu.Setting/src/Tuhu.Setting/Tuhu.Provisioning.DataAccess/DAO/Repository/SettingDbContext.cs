using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Framework.DbCore;

namespace Tuhu.Provisioning.DataAccess.DAO.Repository
{
    public class SettingDbContext : BaseDbContext
    {
        /// <summary>
        /// 默认设置的是Configuration数据库
        /// </summary>
        /// <param name="isReadonly"></param>
        public SettingDbContext(bool isReadonly)
          : base(isReadonly ? "name=Configuration_AlwaysOnRead" : "name=Configuration")
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;

        }

        public SettingDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // dynamic configurationInstance = Activator.CreateInstance(type);
            //  modelBuilder.Configurations.Add( new T1());
            //Assembly.GetExecutingAssembly().Location;
            //Assembly.GetExecutingAssembly().Location;
            string assembleFileName = Assembly.GetExecutingAssembly().Location; //Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");

            Assembly asm = Assembly.LoadFile(assembleFileName);

            foreach (var t in asm.GetTypes().Where(o => o.BaseType != null))
            {
                if (t.BaseType.FullName.Contains("Mapping"))
                    if (t.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>))
                    {
                        dynamic configurationInstance = Activator.CreateInstance(t);
                        modelBuilder.Configurations.Add(configurationInstance);
                    }
            }

            base.OnModelCreating(modelBuilder);
        }

    }

}
