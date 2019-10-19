using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class CarouselMap : EntityTypeConfiguration<Carousel>
    {
        public CarouselMap()
        {
            Property(c => c.DisplayOrder);

            Property(c => c.Title).IsUnicode().IsVariableLength();

            Property(c => c.Description).IsUnicode().IsVariableLength();

            Property(c => c.LinkUrl).IsUnicode().IsVariableLength();

            Ignore(c => c.Slide);
        }
    }
}