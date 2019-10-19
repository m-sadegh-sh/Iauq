using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class LogMap : EntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            Ignore(l => l.Level);
            Property(l => l.LevelShort);

            Property(l => l.Message).IsUnicode().IsVariableLength().IsMaxLength();
            Property(l => l.Stack).IsVariableLength().IsMaxLength();
            Property(l => l.ExceptionType).IsVariableLength().HasMaxLength(1024);
            Property(l => l.RequestUrl).IsVariableLength().IsMaxLength();
            Property(l => l.ReferUrl).IsVariableLength().IsMaxLength();
            Property(l => l.IpAddress).IsVariableLength().HasMaxLength(32);
            Property(l => l.LogDate).IsRequired();
        }
    }
}