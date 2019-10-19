using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            Property(c => c.DisplayOrder);

            Property(c => c.Title).IsUnicode().IsVariableLength();

            HasRequired(c => c.Language).WithMany(l => l.Categories).HasForeignKey(c => c.LanguageId).
                WillCascadeOnDelete(false);

            //HasMany(c => c.Contents).WithRequired(c => c.Category);
            HasOptional(c => c.Parent).WithMany(c => c.Childs).HasForeignKey(c => c.ParentId).WillCascadeOnDelete(false);
        }
    }
}