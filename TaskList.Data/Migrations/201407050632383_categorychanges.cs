namespace TaskList.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class categorychanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectCategories", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.ProjectCategories", "ProjectID", "dbo.Projects");
            DropIndex("dbo.ProjectCategories", new[] { "ProjectID" });
            DropIndex("dbo.ProjectCategories", new[] { "CategoryID" });
            DropTable("dbo.ProjectCategories");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProjectCategories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProjectID = c.Int(nullable: false),
                        CategoryID = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        EditedBy = c.String(),
                        EditDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.ProjectCategories", "CategoryID");
            CreateIndex("dbo.ProjectCategories", "ProjectID");
            AddForeignKey("dbo.ProjectCategories", "ProjectID", "dbo.Projects", "ID", cascadeDelete: true);
            AddForeignKey("dbo.ProjectCategories", "CategoryID", "dbo.Categories", "ID", cascadeDelete: true);
        }
    }
}
