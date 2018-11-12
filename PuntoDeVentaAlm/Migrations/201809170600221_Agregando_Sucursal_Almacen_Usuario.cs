namespace PuntoDeVentaAlm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Agregando_Sucursal_Almacen_Usuario : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ID_SUCURSAL", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "ID_ALMACEN", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ID_ALMACEN");
            DropColumn("dbo.AspNetUsers", "ID_SUCURSAL");
        }
    }
}
