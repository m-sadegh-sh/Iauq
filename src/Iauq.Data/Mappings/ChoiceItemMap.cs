using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class ChoiceItemMap : EntityTypeConfiguration<ChoiceItem>
    {
        public ChoiceItemMap()
        {
            HasRequired(ci => ci.Owner).WithMany(c => c.Items).HasForeignKey(ci => ci.OwnerId).WillCascadeOnDelete(true);

            Property(ci => ci.Text).IsUnicode().IsVariableLength();
        }
    }
}