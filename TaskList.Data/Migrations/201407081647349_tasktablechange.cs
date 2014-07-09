namespace TaskList.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tasktablechange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "UserID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "UserID");
        }
    }
}
