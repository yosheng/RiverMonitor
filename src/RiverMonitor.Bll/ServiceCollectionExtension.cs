using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RiverMonitor.Bll;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        var assembly = typeof(ServiceCollectionExtension).Assembly;
        var types = assembly
            .GetTypes()
            .Where(t =>
                !t.IsGenericType &&
                !t.IsAbstract &&
                t.IsClass &&
                t.Name.EndsWith("Service") &&
                !t.Name.EndsWith("ApiService"))
            .ToList();
        foreach (var type in types)
        {
            var baseType = type.GetInterfaces().FirstOrDefault(t => t.Name == $"I{type.Name}");
            serviceCollection.TryAddScoped(baseType, type);
        }
        return serviceCollection;
    }    
}