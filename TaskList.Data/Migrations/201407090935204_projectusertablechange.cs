namespace TaskList.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class projectusertablechange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectUsers", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ProjectUsers", new[] { "User_Id" });
            DropColumn("dbo.ProjectUsers", "UserID");
            RenameColumn(table: "dbo.ProjectUsers", name: "User_Id", newName: "UserID");
            AlterColumn("dbo.ProjectUsers", "UserID", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ProjectUsers", "UserID", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ProjectUsers", "UserID");
            AddForeignKey("dbo.ProjectUsers", "UserID", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectUsers", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.ProjectUsers", new[] { "UserID" });
            AlterColumn("dbo.ProjectUsers", "UserID", c => c.String(maxLength: 128));
            AlterColumn("dbo.ProjectUsers", "UserID", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.ProjectUsers", name: "UserID", newName: "User_Id");
            AddColumn("dbo.ProjectUsers", "UserID", c => c.Int(nullable: false));
            CreateIndex("dbo.ProjectUsers", "User_Id");
            AddForeignKey("dbo.ProjectUsers", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
