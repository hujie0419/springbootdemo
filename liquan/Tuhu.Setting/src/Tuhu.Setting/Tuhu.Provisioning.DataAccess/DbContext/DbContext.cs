using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Tuhu.Provisioning.DataAccess
{

    public partial class DiscoveryDbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //// Configure Code First to ignore PluralizingTableName convention 
            //// If you keep this convention then the generated tables will have pluralized names. 
            ////modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Types().Configure(c => c.ToTable("tbl_" + c.ClrType.Name));

            //modelBuilder.Entity<Course>()
            //    .HasMany(t => t.Instructors)
            //    .Wi   

            //     modelBuilder.Entity<ArticleCategory>()
            //.HasKey(at => new { at.CategoryId, at.ArticleId });

            //modelBuilder.Entity<ArticleCategory>()              
            //      .HasKey(at => new { at.ArticleId, at.CategoryId });

            //modelBuilder.Entity<Article>()
            //    .HasMany(a => a.ArticleCategorys)
            //    .WithRequired()
            //    .HasForeignKey(a => a.ArticleId);

            //modelBuilder.Entity<Category>()
            //    .HasMany(t => t.ArticleCategorys)
            //    .WithRequired()
            //    .HasForeignKey(t => t.CategoryId);

            //modelBuilder.Entity<ArticleCategory>()
            //.HasMany(at => at.Articles)
            //.WithRequired()
            //.HasForeignKey(at => at.Id);
        }
    }

    public class MigrationsContextFactory : IDbContextFactory<DiscoveryDbContext>
    {
        public DiscoveryDbContext Create()
        {
            return new DiscoveryDbContext();
        }
    }
}
