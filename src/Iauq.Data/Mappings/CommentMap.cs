using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class CommentMap : EntityTypeConfiguration<Comment>
    {
        public CommentMap()
        {
            Property(c => c.IsApproved);

            Ignore(c => c.CommentDate);

            Property(c => c.CommentDateTicks);

            Property(c => c.PositiveRates);

            Property(c => c.NagetiveRates);

            Property(c => c.Title).IsUnicode().IsVariableLength();

            Property(c => c.Body).IsUnicode().IsVariableLength();

            HasOptional(c => c.Parent).WithMany(c => c.Childs).HasForeignKey(c=>c.ParentId).WillCascadeOnDelete(false);

            Property(c => c.CommentorIp).IsOptional().IsVariableLength().HasMaxLength(32);

            //HasOptional(c => c.Commentor).WithMany(u => u.Comments).WillCascadeOnDelete();

            //HasRequired(c => c.Owner).WithMany(c => c.Comments).WillCascadeOnDelete();
        }
    }
}