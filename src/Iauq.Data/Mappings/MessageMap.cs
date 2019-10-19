using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class MessageMap : EntityTypeConfiguration<Message>
    {
        public MessageMap()
        {
            Property(m => m.Id);

            HasRequired(m => m.Owner).WithMany(m => m.Messages).HasForeignKey(m => m.OwnerId).WillCascadeOnDelete(false);

            Property(m => m.SenderName).IsUnicode().IsVariableLength();

            Property(m => m.SenderId).IsUnicode().IsVariableLength();

            Property(m => m.SenderPhone).IsUnicode().IsVariableLength();

            Property(m => m.SenderEmail).IsUnicode().IsVariableLength();

            Ignore(m => m.SentDate);

            Property(m => m.SentTicks);

            Property(m => m.Subject).IsUnicode().IsVariableLength();

            Property(m => m.Body).IsUnicode().IsVariableLength();
        }
    }
}