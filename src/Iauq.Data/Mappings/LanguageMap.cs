using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class LanguageMap : EntityTypeConfiguration<Language>
    {
        public LanguageMap()
        {
            Property(l => l.Name).IsUnicode().IsVariableLength();

            Property(l => l.IsoCode);

            //HasMany(l => l.Categories).WithRequired(c => c.Language);

            //HasMany(l => l.Contents).WithRequired(c => c.Language);
        }
    }
}