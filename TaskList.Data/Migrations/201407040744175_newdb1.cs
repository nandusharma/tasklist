namespace TaskList.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newdb1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "IsDefault");
        }
    }
}
