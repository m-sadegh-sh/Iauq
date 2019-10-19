namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CommentorIpAddedToComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("Comments", "CommentorIp", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("Comments", "CommentorIp");
        }
    }
}
