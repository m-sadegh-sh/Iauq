namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CategoryAddedToRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("Roles", "CategoryId", c => c.Int(nullable: false));
            AddForeignKey("Roles", "CategoryId", "Categories", "Id");
            CreateIndex("Roles", "CategoryId");
        }
        
        public override void Down()
        {
            DropIndex("Roles", new[] { "CategoryId" });
            DropForeignKey("Roles", "CategoryId", "Categories");
            DropColumn("Roles", "CategoryId");
        }
    }
}
