using ShortVideoCutter.Exceptions;
using ShortVideoCutter.Extensions;
using ShortVideoCutter.Interfaces.ModuleInterfaces;
using System.Reflection;

namespace ShortVideoCutter.DI;

public class LittleDI
{
    private Dictionary<Type, List<ServiceData>> _services;

    private List<InitServiceData> _initialBuffer;

    public LittleDI()
    {
        _services = new Dictionary<Type, List<ServiceData>>();
        _initialBuffer = new List<InitServiceData>();
    }

    public void Run()
    {
        var order = new Dictionary<InitServiceData, int>();

        foreach (var initData in _initialBuffer)
        {
            ConstructorInfo[] constructors = initData.InstanseServiceType.GetConstructors();

            var condition = (ConstructorInfo x) =>
                x.GetParameters().Length == 0 ||
                x.GetParameters().All(p => _initialBuffer.Select(x => x.ServiceSignature)
                                              .Contains(p.ParameterType));

            // last with many
            var fullishAvailableCtor = GetFullishAvailableConstructor(initData.InstanseServiceType, condition);

            order.Add(initData, GetCount(fullishAvailableCtor, condition));
        }

        foreach (var initData in order.OrderBy(x => x.Value))
        {
            AddService(initData.Key.ServiceSignature, initData.Key.InstanseServiceType, initData.Key.ServiceLifespan);
        }
    }

    private int GetCount(ConstructorInfo constructor, Func<ConstructorInfo, bool> condition)
    {
        var parametrs = constructor.GetParameters();
        if (parametrs.Length == 0)
            return 0;

        var interfaceToinstanceType = (Type interfaceType) =>
        {
            return _initialBuffer
                .Where(x => x.ServiceSignature == interfaceType)
                .Select(x => x.InstanseServiceType);
        };
        var instanceParams = new List<Type>();
        foreach (var param in parametrs)
        {
            var instType = interfaceToinstanceType(param.ParameterType);
            instanceParams.AddRange(instType);
        }

        return instanceParams.Select(x =>
            GetCount(GetFullishAvailableConstructor(x, condition), condition)).Sum() + 1;
    }

    private ConstructorInfo GetFullishAvailableConstructor(Type type,
        Func<ConstructorInfo, bool> condition)
    {
        var constructors = type.GetConstructors();
        var availableCtors = constructors.Where(condition);
        if (!availableCtors.Any())
        {
            throw new VideoCutterDIException("No constructor is suitable");
        }

        // if exist same length available constructors
        if (availableCtors.Select(x => x.GetParameters().Length).ToHashSet().Count != availableCtors.Count())
        {
            throw new VideoCutterDIException("Exist same length available constructors");
        }
        return availableCtors.OrderBy(x => x.GetParameters().Length).Last();
    }

    public void RegistrateService<TServiceSignature, TInstanseServiceType>(EServiceLifespan lifespan = EServiceLifespan.Singleton)
    {
        _initialBuffer.Add(new InitServiceData(typeof(TServiceSignature), typeof(TInstanseServiceType), lifespan));
    }

    private void AddService(Type serviceSignature, Type instanseServiceType, EServiceLifespan lifespan = EServiceLifespan.Singleton)
    {
        ConstructorInfo[] constructors = instanseServiceType.GetConstructors();

        // last with many
        var fullishAvailableCtor = GetFullishAvailableConstructor(instanseServiceType, IsSuitableCtor);
        var parametars = fullishAvailableCtor.GetParameters().Select(x => GetService(x.ParameterType));

        if (lifespan == EServiceLifespan.AlwaysNew)
        {
            _services.AddItemInListInDict(serviceSignature, new(null, lifespan, instanseServiceType, fullishAvailableCtor));
        }

        var service = (IService)Activator.CreateInstance(instanseServiceType, parametars.ToArray());
        _services.AddItemInListInDict(serviceSignature, new(service, lifespan, instanseServiceType, fullishAvailableCtor));
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
        if (_services.TryGetValue(serviceType, out var list))
        {
            if (list.FirstOrDefault() is ServiceData serviceData)
            {
                return ReturnServiceBaseOnLifeSpan(serviceData);
            }
        }
        return default;
    }

    private IService ReturnServiceBaseOnLifeSpan(ServiceData serviceData)
    {
        return serviceData.Lifespan switch
        {
            EServiceLifespan.Singleton => serviceData.Service,
            _ => CreateNewInstance(serviceData)
        };
    }

    private IService CreateNewInstance(ServiceData serviceType)
    {
        var parametars = serviceType.Constructor.GetParameters().Select(x => GetService(x.ParameterType));
        return (IService)Activator.CreateInstance(serviceType.ServiceType, parametars.ToArray());
    }

    private bool ExistService(Type serviceType)
    {
        return _services.TryGetValue(serviceType, out var list);
    }
}
