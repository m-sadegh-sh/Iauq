using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class ContentMap : EntityTypeConfiguration<Content>
    {
        public ContentMap()
        {
            Property(c => c.IsPublished);

            Property(c => c.ShowNestedContents);

            Property(c => c.IsHot);

            Ignore(c => c.Type);

            Property(c => c.TypeInt);

            Property(c => c.DisplayOrder);

            Ignore(c => c.PublishDate);

            Property(c => c.PublishDateTicks);

            Property(c => c.Rate);

            Property(c => c.PageViews);

            Property(c => c.Title).IsUnicode().IsVariableLength();

            Property(c => c.Abstract).IsUnicode().IsVariableLength();

            Property(c => c.Body).IsUnicode().IsVariableLength();

            Property(c => c.Tags).IsUnicode().IsVariableLength();

            HasOptional(c => c.Category).WithMany(c => c.Contents).HasForeignKey(c => c.CategoryId).WillCascadeOnDelete(false);

            HasOptional(c => c.Parent).WithMany(c => c.Childs).HasForeignKey(c => c.ParentId).WillCascadeOnDelete(false);

            //HasRequired(c => c.Author).WithMany(u => u.Contents);

            HasMany(c => c.Comments).WithRequired(c => c.Owner).WillCascadeOnDelete(false);
        }
    }
}