using ShortVideoCutter.Extensions;
using ShortVideoCutter.Interfaces;
using System.Reflection;

namespace ShortVideoCutter.DI;

public class LittleDI
{
    private Dictionary<Type, List<ServiceData>> services = new Dictionary<Type, List<ServiceData>>();

    public void AddService(Type serviceSignature, Type instanseServiceType, ServiceLifespan lifespan = ServiceLifespan.Singleton)
    {


        ConstructorInfo[] constructors = instanseServiceType.GetConstructors();

        // last with many
        var availableCtors = constructors.Where(IsSuitableCtor);

        if (!availableCtors.Any())
        {
            throw new Exception("No constructor is suitable");
        }

        // if exist same length available constructors
        if (availableCtors.Select(x => x.GetParameters().Length).ToHashSet().Count != availableCtors.Count())
        {
            throw new Exception("Exist same length available constructors");
        }
        var fullishAvailableCtor = availableCtors.OrderBy(x => x.GetParameters().Length).Last();
        var parametars = fullishAvailableCtor.GetParameters().Select(x => GetService(x.ParameterType));

        if (lifespan == ServiceLifespan.AlwaysNew)
        {
            services.AddItemInListInDict(serviceSignature, new(null, lifespan, instanseServiceType, fullishAvailableCtor));
        }

        var service = (IService)Activator.CreateInstance(instanseServiceType, parametars.ToArray());
        services.AddItemInListInDict(serviceSignature, new(service, lifespan, instanseServiceType, fullishAvailableCtor));
    }

    private bool IsSuitableCtor(ConstructorInfo constructorInfo)
    {
        if (constructorInfo.GetParameters() is { } parameters)
        {
            return parameters.Length == 0 || parameters.All(x => ExistService(x.ParameterType));
        }

        return false;
    }

    public TService GetService<TService>() where TService : IService
    {
        return (TService)GetService(typeof(TService));
    }

    public IService GetService(Type serviceType)
    {
        if (services.TryGetValue(serviceType, out var list))
        {
            if (list.FirstOrDefault() is ServiceData serviceData)
            {
                return ReturnServiceBaseOnLifeSpan(serviceData);
            }
        }
        return default;
    }

    public IService ReturnServiceBaseOnLifeSpan(ServiceData serviceData)
    {
        return serviceData.Lifespan switch
        {
            ServiceLifespan.Singleton => serviceData.Service,
            _ => CreateNewInstance(serviceData)
        };
    }

    public IService CreateNewInstance(ServiceData serviceType)
    {
        var parametars = serviceType.Constructor.GetParameters().Select(x => GetService(x.ParameterType));
        return (IService)Activator.CreateInstance(serviceType.ServiceType, parametars.ToArray());
    }

    private bool ExistService(Type serviceType)
    {
        return services.TryGetValue(serviceType, out var list);
    }
}
