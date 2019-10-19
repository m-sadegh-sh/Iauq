namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RoleCategoryToOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Roles", "CategoryId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Roles", "CategoryId", c => c.Int(nullable: false));
        }
    }
}
