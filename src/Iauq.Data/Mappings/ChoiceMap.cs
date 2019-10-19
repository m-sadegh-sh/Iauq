using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class ChoiceMap : EntityTypeConfiguration<Choice>
    {
        public ChoiceMap()
        {
            HasRequired(c => c.Owner).WithMany(p => p.Choices).HasForeignKey(c => c.OwnerId).WillCascadeOnDelete(true);

            Property(c => c.Title).IsUnicode().IsVariableLength();
            Property(c => c.Description).IsUnicode().IsMaxLength();

            Property(c => c.IsMultiSelection);
        }
    }
}