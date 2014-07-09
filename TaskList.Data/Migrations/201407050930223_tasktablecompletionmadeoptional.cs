namespace TaskList.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tasktablecompletionmadeoptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tasks", "Completion", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tasks", "Completion", c => c.DateTime(nullable: false));
        }
    }
}
