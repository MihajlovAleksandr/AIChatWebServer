using System.Reflection;
using AIChatWebServer.Utils.Interfaces.Mapper;
using AIChatWebServer.Utils.Implementations.Mappers;

namespace AIChatWebServer.Utils.Implementations
{
    public static class MapperServiceCollectionExtensions
    {
        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            RegisterMappers(services, assembly);
            RegisterCollectionMappers(services);

            return services;
        }

        private static void RegisterMappers(IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface);

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();

                foreach (var i in interfaces)
                {
                    if (!i.IsGenericType)
                        continue;

                    var definition = i.GetGenericTypeDefinition();

                    if (definition == typeof(IMapper<,,>) ||
                        definition == typeof(IRequestMapper<,>) ||
                        definition == typeof(IResponseMapper<,>))
                    {
                        services.AddScoped(i, type);
                    }
                }
            }
        }

        private static void RegisterCollectionMappers(IServiceCollection services)
        {
            services.AddScoped(typeof(ICollectionRequestMapper<,>), typeof(CollectionRequestMapper<,>));
            services.AddScoped(typeof(ICollectionResponseMapper<,>), typeof(CollectionResponseMapper<,>));
            services.AddScoped(typeof(ICollectionMapper<,,>), typeof(CollectionMapper<,,>));
        }
    }
}