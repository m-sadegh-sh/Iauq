namespace Iauq.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PollingAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Answers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SelectedItemId = c.Int(nullable: false),
                        AnswererId = c.Int(),
                        IpAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("ChoiceItems", t => t.SelectedItemId, cascadeDelete: true)
                .ForeignKey("Users", t => t.AnswererId)
                .Index(t => t.SelectedItemId)
                .Index(t => t.AnswererId);
            
            CreateTable(
                "ChoiceItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OwnerId = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Choices", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "Choices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OwnerId = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        IsMultiSelection = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Polls", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "Polls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        StartActiveTimeTicks = c.Long(),
                        EndActiveTimeTicks = c.Long(),
                        Title = c.String(),
                        Body = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("Choices", new[] { "OwnerId" });
            DropIndex("ChoiceItems", new[] { "OwnerId" });
            DropIndex("Answers", new[] { "AnswererId" });
            DropIndex("Answers", new[] { "SelectedItemId" });
            DropForeignKey("Choices", "OwnerId", "Polls");
            DropForeignKey("ChoiceItems", "OwnerId", "Choices");
            DropForeignKey("Answers", "AnswererId", "Users");
            DropForeignKey("Answers", "SelectedItemId", "ChoiceItems");
            DropTable("Polls");
            DropTable("Choices");
            DropTable("ChoiceItems");
            DropTable("Answers");
        }
    }
}
