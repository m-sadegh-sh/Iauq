using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class SiteMap : EntityTypeConfiguration<Site>
    {
        public SiteMap()
        {
            Property(s => s.Id);

            Property(s => s.Key).IsVariableLength().IsRequired();

            Property(s => s.OwnerId).IsVariableLength().IsRequired();

            HasOptional(s => s.Parent).WithMany(s => s.Childs).HasForeignKey(s => s.ParentId).WillCascadeOnDelete(false);

            //HasOptional(s => s.Category).WithMany(c => c.Contents).HasForeignKey(s => s.CategoryId).WillCascadeOnDelete(
            //    false);

            Property(s => s.Title).IsUnicode().IsVariableLength();

            Property(s => s.Description).IsUnicode().IsVariableLength();

            Ignore(s => s.Type);

            Property(s => s.TypeShort);

            Property(s => s.SourceId).IsUnicode().IsVariableLength();

            Ignore(s => s.LastLoginDate);

            Property(s => s.LastLoginTicks);
            
            Property(s => s.ThemeId);

            Property(s => s.ShowMessageBox);

            Property(s => s.AdminId);

            Property(s => s.Email);
        }
    }
}