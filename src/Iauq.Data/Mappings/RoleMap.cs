using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            Property(r => r.Name).IsUnicode().IsVariableLength();

            HasOptional(r => r.Category).WithMany(r => r.Roles).HasForeignKey(r => r.CategoryId).WillCascadeOnDelete(
                false);

            //HasMany(r => r.Users).WithMany(u => u.Roles);
        }
    }
}