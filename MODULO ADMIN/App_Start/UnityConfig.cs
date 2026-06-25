using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using DAL.Servicios;
using DAL.Servicios.ServiciosImp;
using ENTIDADES.RepositoryImp;
using ENTIDADES.Repository;
using ENTIDADES;


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
                container.RegisterType<IMarcaRepository, MarcaRepositoryImp>();
                container.RegisterType<IProductoRepository, ProductoRepositoryImp>();
                container.RegisterType<IVentaRepository, VentaRepositoryImp>();
                container.RegisterType<IUsuarioRepository, UsuarioRepositoryImp>();
                container.RegisterType<IClienteRepository, ClienteRepositoryImp>();
                container.RegisterType <IUserService, UserServiceImp>();
                container.RegisterType <IMarcaService, MarcaServiceImp>();
                container.RegisterType <ICategoriasService, CategoriasServiceImp>();
                container.RegisterType <IProductoService, ProductoServiceImp>();
                container.RegisterType <IVentaService, VentaServiceImp>();
                container.RegisterType <IClienteService, ClienteServiceImp>();


            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}