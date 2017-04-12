namespace ContosoWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveInstructorFirstNameRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Instructor", "FirstName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Instructor", "FirstName", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
