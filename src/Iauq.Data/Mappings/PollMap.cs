using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class PollMap : EntityTypeConfiguration<Poll>
    {
        public PollMap()
        {
            Property(p => p.IsActive);

            Ignore(p => p.CreateDate);
            Property(p => p.CreateDateTicks);

            Property(p => p.Title).IsUnicode().IsVariableLength();
            Property(p => p.Description).IsUnicode().IsMaxLength();
        }
    }
}