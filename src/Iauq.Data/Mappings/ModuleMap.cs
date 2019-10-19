using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class ModuleMap : EntityTypeConfiguration<Module>
    {
        public ModuleMap()
        {
            Property(m => m.Id);

            HasRequired(m => m.Owner).WithMany(m => m.Modules).HasForeignKey(m => m.OwnerId).WillCascadeOnDelete(false);

            Property(m => m.Title).IsUnicode().IsVariableLength();

            Property(m => m.Description).IsUnicode().IsVariableLength();

            Property(m => m.Content).IsUnicode().IsVariableLength();

            Ignore(m => m.Position);

            Property(m => m.PositionShort);

            Property(m => m.Order);

            Property(m => m.Visible);

            Property(m => m.Deleted);
        }
    }
}