using System.Data.Entity.Migrations;

namespace Iauq.Data.Migrations
{
    public partial class ContentCategoryToOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Contents", "CategoryId", c => c.Int());
        }

        public override void Down()
        {
            AlterColumn("Contents", "CategoryId", c => c.Int(nullable: false));
        }
    }
}