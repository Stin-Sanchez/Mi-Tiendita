using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using DAL.Servicios;
using ENTIDADES.RepositoryImp;
using ENTIDADES.Repository;
using ENTIDADES;
using DAL.Servicios.ServiciosImp;

namespace MODULO_CLIENTE
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();


            container.RegisterType(typeof(ICrudRepository<>), typeof(RepositoryImp<>));
            container.RegisterType<IClienteRepository, ClienteRepositoryImp>();
            container.RegisterType<IClienteService, ClienteServiceImp>();
            container.RegisterType<IProductoRepository, ProductoRepositoryImp>();
            container.RegisterType<IVentaRepository, VentaRepositoryImp>();
            container.RegisterType<IMarcaService, MarcaServiceImp>();
            container.RegisterType<ICategoriasService, CategoriasServiceImp>();
            container.RegisterType<IProductoService, ProductoServiceImp>();
            container.RegisterType<IVentaService, VentaServiceImp>();
           


            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}