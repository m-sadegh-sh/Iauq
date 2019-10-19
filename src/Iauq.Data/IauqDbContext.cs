using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using Iauq.Core.Domain;
using Iauq.Data.Mappings;

namespace Iauq.Data
{
    public class IauqDbContext : DbContext, IUnitOfWork
    {
        public IauqDbContext()
        {
            Configuration.AutoDetectChangesEnabled = false;
        }

        #region IUnitOfWork Members

        public new virtual IDbSet<TEntity> Set<TEntity>() where TEntity : EntityBase
        {
            return base.Set<TEntity>();
        }

        public int GetNextTableIdentity(string tableName)
        {
            using (DbCommand cmd = Database.Connection.CreateCommand())
            {
                bool manuallyOpened = false;
                if (Database.Connection.State != ConnectionState.Open)
                {
                    manuallyOpened = true;
                    Database.Connection.Open();
                }

                cmd.CommandText = "select IDENT_CURRENT('" + tableName + "')";
                cmd.CommandType = CommandType.Text;

                var id = cmd.ExecuteScalar();

                if (manuallyOpened)
                    Database.Connection.Close();

                var intId = Convert.ToInt32(id);

                return ++intId;
            }
        }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CommentMap());
            modelBuilder.Configurations.Add(new ContentMap());
            modelBuilder.Configurations.Add(new CarouselMap());
            modelBuilder.Configurations.Add(new LanguageMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new LogMap());
            modelBuilder.Configurations.Add(new FileMap());
            modelBuilder.Configurations.Add(new TemplateMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}