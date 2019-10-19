namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedCommentToEnableAnonymusCommenting : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Comments", "CommentorId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Comments", "CommentorId", c => c.Int(nullable: false));
        }
    }
}
