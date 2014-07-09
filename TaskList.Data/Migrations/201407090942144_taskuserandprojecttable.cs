namespace TaskList.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskuserandprojecttable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Projects", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Projects", new[] { "User_Id" });
            RenameColumn(table: "dbo.Projects", name: "User_Id", newName: "UserID");
            AlterColumn("dbo.Projects", "UserID", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Projects", "UserID");
            AddForeignKey("dbo.Projects", "UserID", "dbo.AspNetUsers", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Projects", new[] { "UserID" });
            AlterColumn("dbo.Projects", "UserID", c => c.String(maxLength: 128));
            RenameColumn(table: "dbo.Projects", name: "UserID", newName: "User_Id");
            CreateIndex("dbo.Projects", "User_Id");
            AddForeignKey("dbo.Projects", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
