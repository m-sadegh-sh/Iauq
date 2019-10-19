using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class SeoMetadataMap : ComplexTypeConfiguration<SeoMetadata>
    {
        public SeoMetadataMap()
        {
            Property(sm => sm.SeoTitle).IsUnicode().IsVariableLength().IsOptional();
            Property(sm => sm.SeoSlug).IsUnicode().IsVariableLength().IsOptional();
            Property(sm => sm.SeoKeywords).IsUnicode().IsVariableLength().IsOptional();
            Property(sm => sm.SeoDescription).IsUnicode().IsVariableLength().IsOptional();
        }
    }
}