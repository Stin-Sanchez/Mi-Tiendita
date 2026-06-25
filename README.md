# Mi Tiendita

E-commerce fullstack construido con ASP.NET MVC 5, dividido en dos módulos independientes: un panel de administración y una tienda para clientes.

---

## Stack tecnológico

| Capa | Tecnología |
|---|---|
| Backend | ASP.NET MVC 5 / .NET Framework 4.8 |
| ORM | Entity Framework 6 Code First |
| Base de datos | SQL Server (LocalDB / SQLEXPRESS) |
| Inyección de dependencias | Unity 5 |
| UI Admin | SB Admin 2 + Bootstrap 4 + Chart.js |
| UI Cliente | Bootstrap 4 |
| Autenticación | Forms Authentication + SHA-256 |
| Email | SMTP Gmail (`SmtpClient`) |

---

## Estructura del proyecto

```
Mi-Tiendita/
├── ENTIDADES/          # Modelos EF, DbContext, Repositories, Migrations
├── DAL/                # Servicios e implementaciones (lógica de negocio)
├── MODULO ADMIN/       # Panel administrativo (MVC 5)
└── MODULO CLIENTE/     # Tienda pública (MVC 5)
```

### Capas

```
MODULO ADMIN / MODULO CLIENTE
        │
       DAL  (IService → ServiceImp)
        │
    ENTIDADES  (IRepository → RepositoryImp → EF DbContext → SQL Server)
```

---

## Módulos del panel de administración

| Módulo | Ruta | Descripción |
|---|---|---|
| Dashboard | `/Home` | KPIs, gráficas de ventas, métricas del negocio |
| Usuarios | `/Home/Usuarios` | CRUD de usuarios administradores |
| Clientes | `/Clientes` | Gestión de clientes registrados en la tienda |
| Categorías | `/Mantenedor/Categorias` | CRUD de categorías de productos |
| Marcas | `/Mantenedor/Marca` | CRUD de marcas |
| Productos | `/Mantenedor/Producto` | CRUD de productos con imagen |
| Inventario | `/Inventario` | Ajuste de stock (entradas y salidas) |
| Pedidos | `/Pedidos` | Historial de ventas con filtros y exportación |
| Reportes | `/Reportes` | Análisis de ventas por período, top productos y clientes |
| Carga Masiva | `/CargaMasiva` | Importación de productos, categorías y marcas por CSV |

### Módulo cliente

| Módulo | Ruta | Descripción |
|---|---|---|
| Tienda | `/Tienda` | Catálogo de productos con carrito |
| Detalle | `/Tienda/DetalleProducto` | Detalle de producto |
| Acceso | `/Acceso` | Login, registro, recuperación de clave |

---

## Modelo de datos principal

```
USUARIOS (admin)        CLIENTES (tienda)
                              │
                            VENTAS
                              │
                       DETALLE_VENTAS ──── PRODUCTOS ──── CATEGORIAS
                                                    └──── MARCAS
                         CARRITO ──── CLIENTES
                                 └──── PRODUCTOS

DISTRITO ──── PROVINCIA ──── DEPARTAMENTO   (jerarquía geográfica para envíos)
```

---

## Configuración local

### Pre-requisitos

- Visual Studio 2019 o superior
- SQL Server / SQLEXPRESS
- .NET Framework 4.8

### 1. Base de datos

```sql
CREATE DATABASE DBCARRITO;
```

Luego ejecuta las migraciones desde la Package Manager Console (proyecto **ENTIDADES**):

```powershell
Update-Database
```

O aplica los scripts en `ENTIDADES/Migrations/` manualmente.

### 2. Connection string

Edita `ENTIDADES/App.config`, `MODULO ADMIN/Web.config` y `MODULO CLIENTE/Web.config`:

```xml
<connectionStrings>
  <add name="ModelContext"
       connectionString="data source=.\SQLEXPRESS;initial catalog=DBCARRITO;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 3. Credenciales SMTP (correo)

Crea el archivo `MODULO CLIENTE/credenciales.config` (está en `.gitignore`):

```xml
<appSettings>
  <add key="CorreoEmisor" value="tucorreo@gmail.com" />
  <add key="ClaveEmisor"  value="tu-app-password-gmail" />
</appSettings>
```

> Requiere una **App Password** de Google, no la clave de cuenta. Actívala en: Cuenta Google → Seguridad → Verificación en dos pasos → Contraseñas de aplicación.

### 4. Ejecutar

Establece **MODULO ADMIN** y **MODULO CLIENTE** como proyectos de inicio múltiple en Visual Studio y presiona F5.

---

## Seguridad

- Contraseñas encriptadas con **SHA-256** antes de persistir.
- `credenciales.config` excluido de git vía `.gitignore`.
- Connection strings usan **Integrated Security** — sin usuario ni contraseña en texto plano.
- Rutas del panel admin protegidas con `[Authorize]` + Forms Authentication.
- Restablecimiento de clave por correo con token de un solo uso.

---

## Características destacadas

- **Carga masiva CSV**: importa productos, categorías y marcas desde plantillas descargables sin código adicional.
- **Reportes dinámicos**: ventas por período, top 10 productos y clientes más frecuentes con Chart.js.
- **Inventario**: ajuste de stock con entradas y salidas, alertas visuales por stock crítico (≤ 5 unidades).
- **Dashboard**: KPIs en tiempo real, gráfica de área de ventas, donut por categoría, estado del sistema.
- **Exportación Excel**: historial de ventas exportable desde el módulo Pedidos.

---

## Ramas

| Rama | Descripción |
|---|---|
| `main` | Producción estable |
| `testing` | Desarrollo activo |
