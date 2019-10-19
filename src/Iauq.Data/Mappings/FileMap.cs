using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class FileMap : EntityTypeConfiguration<File>
    {
        public FileMap()
        {
            Property(c => c.Guid);

            Property(c => c.IsPublished);

            Ignore(c => c.AccessMode);

            Property(c => c.AccessModeShort);

            Ignore(c => c.CreateDate);

            Property(c => c.CreateDateTicks);

            Property(c => c.AccessCount);

            Property(c => c.Name).IsUnicode().IsVariableLength();

            Property(c => c.ContentType).IsVariableLength();

            Property(c => c.Size);

            HasOptional(c => c.Parent).WithMany(c => c.Childs).HasForeignKey(c => c.ParentId).WillCascadeOnDelete(false);
        }
    }
}