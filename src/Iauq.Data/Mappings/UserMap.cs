using System.Data.Entity.ModelConfiguration;
using Iauq.Core.Domain;

namespace Iauq.Data.Mappings
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            Property(u => u.UserName).IsUnicode().IsVariableLength();

            Property(u => u.Password).IsUnicode().IsVariableLength();

            Property(u => u.Salt);

            Property(u => u.SecurityToken).IsVariableLength().IsOptional();

            Property(u => u.Email).IsVariableLength();

            HasMany(u => u.Roles).WithMany(r => r.Users).Map(mtmamc =>
                                                                 {
                                                                     mtmamc.ToTable("UsersToRoles");
                                                                     mtmamc.MapLeftKey("UserId");
                                                                     mtmamc.MapRightKey("RoleId");
                                                                 });

            HasMany(u => u.Contents).WithRequired(c => c.Author).HasForeignKey(c => c.AuthorId).WillCascadeOnDelete(false);

            HasMany(u => u.Files).WithRequired(c => c.Uploader).HasForeignKey(c => c.UploaderId).WillCascadeOnDelete(false);

            HasMany(u => u.Comments).WithOptional(c => c.Commentor).HasForeignKey(c => c.CommentorId).WillCascadeOnDelete(false);
        }
    }
}