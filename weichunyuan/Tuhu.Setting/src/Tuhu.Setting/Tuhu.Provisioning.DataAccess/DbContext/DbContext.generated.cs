 

using System.Data.Entity;
using Tuhu.Framework.DbCore;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;
namespace  Tuhu.Provisioning.DataAccess
{
        [ConnectionStringName("Tuhu_Discovery_Db")]
        public partial class DiscoveryDbContext : BaseDbContext
        {        
            public DiscoveryDbContext() : base("name=Tuhu_Discovery_Db") { }
            public DiscoveryDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

		            private DbSet<Article> _article;
            public DbSet<Article> Article
            {
                get { return _article; }
                set { _article = new TuhuDbSet<Article>(value); }
            }
	                private DbSet<ArticleChangeLog> _articleChange_Log;
            public DbSet<ArticleChangeLog> ArticleChange_Log
            {
                get { return _articleChange_Log; }
                set { _articleChange_Log = new TuhuDbSet<ArticleChangeLog>(value); }
            }
	                private DbSet<ArticleFavorite> _articleFavorite;
            public DbSet<ArticleFavorite> ArticleFavorite
            {
                get { return _articleFavorite; }
                set { _articleFavorite = new TuhuDbSet<ArticleFavorite>(value); }
            }
	                private DbSet<ArticleCategory> _articleCategory;
            public DbSet<ArticleCategory> ArticleCategory
            {
                get { return _articleCategory; }
                set { _articleCategory = new TuhuDbSet<ArticleCategory>(value); }
            }
	                private DbSet<Category> _category;
            public DbSet<Category> Category
            {
                get { return _category; }
                set { _category = new TuhuDbSet<Category>(value); }
            }
	                private DbSet<CategoryFavorite> _categoryFavorite;
            public DbSet<CategoryFavorite> CategoryFavorite
            {
                get { return _categoryFavorite; }
                set { _categoryFavorite = new TuhuDbSet<CategoryFavorite>(value); }
            }
	                private DbSet<UserOperation> _userOperation;
            public DbSet<UserOperation> UserOperation
            {
                get { return _userOperation; }
                set { _userOperation = new TuhuDbSet<UserOperation>(value); }
            }
	                private DbSet<Comment> _comment;
            public DbSet<Comment> Comment
            {
                get { return _comment; }
                set { _comment = new TuhuDbSet<Comment>(value); }
            }
	            }
    }