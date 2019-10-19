using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class TemplateMap : EntityTypeConfiguration<Template>
    {
        public TemplateMap()
        {
            Property(t => t.Title).IsUnicode().IsVariableLength().IsMaxLength();
            Property(t => t.Description).IsVariableLength().IsMaxLength().IsUnicode();
            Property(t => t.Body).IsVariableLength().IsMaxLength().IsUnicode();
        }
    }
}