using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using DAL.Servicios;
using ENTIDADES.RepositoryImp;
using ENTIDADES.Repository;
using ENTIDADES;
using DAL.Servicios;

namespace MODULO_ADMIN
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
                container.RegisterType(typeof(ICrudRepository<>), typeof(RepositoryImp<>));
                container.RegisterType<IUsuarioRepository, UsuarioRepositoryImp>();
                container.RegisterType<IProductoRepository, ProductoRepositoryImp>();
                container.RegisterType<IVentaRepository, VentaRepositoryImp>();
                container.RegisterType<IUsuarioRepository, UsuarioRepositoryImp>();
                container.RegisterType <IUserService, UserServiceImp>();
                container.RegisterType <IMarcaService, MarcaServiceImp>();
                container.RegisterType <ICategoriasService, CategoriasServiceImp>();
                container.RegisterType <IProductoService, ProductoServiceImp>();
                container.RegisterType <IVentaService, VentaServiceImp>();


            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}