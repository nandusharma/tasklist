namespace TaskList.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tasktablechangeuserid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tasks", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Tasks", new[] { "User_Id" });
            DropColumn("dbo.Tasks", "UserID");
            RenameColumn(table: "dbo.Tasks", name: "User_Id", newName: "UserID");
            AlterColumn("dbo.Tasks", "UserID", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Tasks", "UserID", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Tasks", "UserID");
            AddForeignKey("dbo.Tasks", "UserID", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Tasks", new[] { "UserID" });
            AlterColumn("dbo.Tasks", "UserID", c => c.String(maxLength: 128));
            AlterColumn("dbo.Tasks", "UserID", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Tasks", name: "UserID", newName: "User_Id");
            AddColumn("dbo.Tasks", "UserID", c => c.Int(nullable: false));
            CreateIndex("dbo.Tasks", "User_Id");
            AddForeignKey("dbo.Tasks", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
