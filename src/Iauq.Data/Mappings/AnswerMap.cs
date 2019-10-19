using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class AnswerMap : EntityTypeConfiguration<Answer>
    {
        public AnswerMap()
        {
            Property(p => p.IpAddress).IsVariableLength().IsOptional().HasMaxLength(32);

            HasRequired(a => a.SelectedItem).WithMany(ci => ci.Answers).HasForeignKey(a => a.SelectedItemId).
                WillCascadeOnDelete(true);

            HasOptional(a => a.Answerer).WithMany(u => u.Answers).HasForeignKey(a => a.AnswererId).WillCascadeOnDelete(
                false);
        }
    }
}