namespace DocFingerPrinterBeta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOriginalAndMarkedBinariesInSameImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Images", "OriginalImageBinary", c => c.Binary());
            AddColumn("dbo.Images", "MarkedImageBinary", c => c.Binary());
            DropColumn("dbo.Images", "imageBinary");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Images", "imageBinary", c => c.Binary());
            DropColumn("dbo.Images", "MarkedImageBinary");
            DropColumn("dbo.Images", "OriginalImageBinary");
        }
    }
}
