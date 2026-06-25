namespace ENTIDADES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CARRITO",
                c => new
                    {
                        ID_CARRITO = c.Int(nullable: false, identity: true),
                        ID_CLIENTE = c.Int(),
                        ID_PRODUCTO = c.Int(),
                        CANTIDAD = c.Int(),
                        FECHA_CREACION = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_CARRITO)
                .ForeignKey("dbo.CLIENTES", t => t.ID_CLIENTE)
                .ForeignKey("dbo.PRODUCTOS", t => t.ID_PRODUCTO)
                .Index(t => t.ID_CLIENTE)
                .Index(t => t.ID_PRODUCTO);
            
            CreateTable(
                "dbo.CLIENTES",
                c => new
                    {
                        ID_CLIENTE = c.Int(nullable: false, identity: true),
                        NOMBRE = c.String(nullable: false, maxLength: 50, unicode: false),
                        APELLIDO = c.String(nullable: false, maxLength: 50, unicode: false),
                        CORREO = c.String(nullable: false, maxLength: 100, unicode: false),
                        CLAVE = c.String(nullable: false, maxLength: 150, unicode: false),
                        ACTIVO = c.Boolean(nullable: false),
                        RESTABLECER = c.Boolean(),
                        FECHA_CREACION = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_CLIENTE);
            
            CreateTable(
                "dbo.VENTAS",
                c => new
                    {
                        ID_VENTA = c.Int(nullable: false, identity: true),
                        ID_CLIENTE = c.Int(),
                        TOTAL_PRODUCTOS = c.Int(),
                        MONTO_TOTAL = c.Decimal(nullable: false, precision: 10, scale: 2),
                        CONTACTO = c.String(maxLength: 100, unicode: false),
                        ID_DISTRITO = c.String(maxLength: 50, unicode: false),
                        TELEFONO = c.String(maxLength: 25, unicode: false),
                        DIRECCION = c.String(maxLength: 200, unicode: false),
                        ID_TRANSACCION = c.String(maxLength: 50, unicode: false),
                        FECHA_VENTA = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_VENTA)
                .ForeignKey("dbo.CLIENTES", t => t.ID_CLIENTE)
                .Index(t => t.ID_CLIENTE);
            
            CreateTable(
                "dbo.DETALLE_VENTAS",
                c => new
                    {
                        ID_DETALLE = c.Int(nullable: false, identity: true),
                        ID_VENTA = c.Int(),
                        ID_PRODUCTO = c.Int(),
                        CANTIDAD = c.Int(),
                        TOTAL = c.Decimal(nullable: false, precision: 10, scale: 2),
                    })
                .PrimaryKey(t => t.ID_DETALLE)
                .ForeignKey("dbo.PRODUCTOS", t => t.ID_PRODUCTO)
                .ForeignKey("dbo.VENTAS", t => t.ID_VENTA)
                .Index(t => t.ID_VENTA)
                .Index(t => t.ID_PRODUCTO);
            
            CreateTable(
                "dbo.PRODUCTOS",
                c => new
                    {
                        ID_PRODUCTO = c.Int(nullable: false, identity: true),
                        NOMBRE = c.String(nullable: false, maxLength: 50, unicode: false),
                        DESCRIPCION = c.String(nullable: false, maxLength: 100, unicode: false),
                        ID_MARCA = c.Int(),
                        ID_CATEGORIA = c.Int(),
                        PRECIO = c.Decimal(nullable: false, precision: 10, scale: 2),
                        STOCK = c.Int(),
                        RUTA_IMAGEN = c.String(maxLength: 100, unicode: false),
                        NOMBRE_IMAGEN = c.String(maxLength: 100, unicode: false),
                        ACTIVO = c.Boolean(nullable: false),
                        FECHA_CREACION = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_PRODUCTO)
                .ForeignKey("dbo.CATEGORIAS", t => t.ID_CATEGORIA)
                .ForeignKey("dbo.MARCAS", t => t.ID_MARCA)
                .Index(t => t.ID_MARCA)
                .Index(t => t.ID_CATEGORIA);
            
            CreateTable(
                "dbo.CATEGORIAS",
                c => new
                    {
                        ID_CATEGORIA = c.Int(nullable: false, identity: true),
                        NOMBRE = c.String(nullable: false, maxLength: 50, unicode: false),
                        DESCRIPCION = c.String(nullable: false, maxLength: 100, unicode: false),
                        ACTIVO = c.Boolean(nullable: false),
                        FECHA_CREACION = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_CATEGORIA);
            
            CreateTable(
                "dbo.MARCAS",
                c => new
                    {
                        ID_MARCA = c.Int(nullable: false, identity: true),
                        NOMBRE = c.String(nullable: false, maxLength: 50, unicode: false),
                        DESCRIPCION = c.String(nullable: false, maxLength: 100, unicode: false),
                        ACTIVO = c.Boolean(nullable: false),
                        FECHA_CREACION = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_MARCA);
            
            CreateTable(
                "dbo.DEPARTAMENTO",
                c => new
                    {
                        ACTIVO = c.Boolean(nullable: false),
                        ID_DEPARTAMENTO = c.String(maxLength: 4, unicode: false),
                        DESCRIPCION = c.String(maxLength: 50, unicode: false),
                        FECHA_CREACION = c.DateTime(),
                    })
                .PrimaryKey(t => t.ACTIVO);
            
            CreateTable(
                "dbo.DISTRITO",
                c => new
                    {
                        ACTIVO = c.Boolean(nullable: false),
                        ID_DISTRITO = c.String(maxLength: 6, unicode: false),
                        ID_PROVINCIA = c.String(maxLength: 4, unicode: false),
                        ID_DEPARTAMENTO = c.String(maxLength: 2, unicode: false),
                        DESCRIPCION = c.String(maxLength: 50, unicode: false),
                        FECHA_CREACION = c.DateTime(),
                    })
                .PrimaryKey(t => t.ACTIVO);
            
            CreateTable(
                "dbo.PROVINCIA",
                c => new
                    {
                        ACTIVO = c.Boolean(nullable: false),
                        ID_PROVINCIA = c.String(maxLength: 4, unicode: false),
                        ID_DEPARTAMENTO = c.String(maxLength: 2, unicode: false),
                        DESCRIPCION = c.String(maxLength: 50, unicode: false),
                        FECHA_CREACION = c.DateTime(),
                    })
                .PrimaryKey(t => t.ACTIVO);
            
            CreateTable(
                "dbo.USUARIOS",
                c => new
                    {
                        ID_USUARIO = c.Int(nullable: false, identity: true),
                        NOMBRE = c.String(nullable: false, maxLength: 50, unicode: false),
                        APELLIDO = c.String(nullable: false, maxLength: 50, unicode: false),
                        CORREO = c.String(nullable: false, maxLength: 100, unicode: false),
                        CLAVE = c.String(nullable: false, maxLength: 150, unicode: false),
                        RESTABLECER = c.Boolean(),
                        ACTIVO = c.Boolean(nullable: false),
                        FECHA_CREACION = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID_USUARIO);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DETALLE_VENTAS", "ID_VENTA", "dbo.VENTAS");
            DropForeignKey("dbo.PRODUCTOS", "ID_MARCA", "dbo.MARCAS");
            DropForeignKey("dbo.DETALLE_VENTAS", "ID_PRODUCTO", "dbo.PRODUCTOS");
            DropForeignKey("dbo.PRODUCTOS", "ID_CATEGORIA", "dbo.CATEGORIAS");
            DropForeignKey("dbo.CARRITO", "ID_PRODUCTO", "dbo.PRODUCTOS");
            DropForeignKey("dbo.VENTAS", "ID_CLIENTE", "dbo.CLIENTES");
            DropForeignKey("dbo.CARRITO", "ID_CLIENTE", "dbo.CLIENTES");
            DropIndex("dbo.PRODUCTOS", new[] { "ID_CATEGORIA" });
            DropIndex("dbo.PRODUCTOS", new[] { "ID_MARCA" });
            DropIndex("dbo.DETALLE_VENTAS", new[] { "ID_PRODUCTO" });
            DropIndex("dbo.DETALLE_VENTAS", new[] { "ID_VENTA" });
            DropIndex("dbo.VENTAS", new[] { "ID_CLIENTE" });
            DropIndex("dbo.CARRITO", new[] { "ID_PRODUCTO" });
            DropIndex("dbo.CARRITO", new[] { "ID_CLIENTE" });
            DropTable("dbo.USUARIOS");
            DropTable("dbo.PROVINCIA");
            DropTable("dbo.DISTRITO");
            DropTable("dbo.DEPARTAMENTO");
            DropTable("dbo.MARCAS");
            DropTable("dbo.CATEGORIAS");
            DropTable("dbo.PRODUCTOS");
            DropTable("dbo.DETALLE_VENTAS");
            DropTable("dbo.VENTAS");
            DropTable("dbo.CLIENTES");
            DropTable("dbo.CARRITO");
        }
    }
}
