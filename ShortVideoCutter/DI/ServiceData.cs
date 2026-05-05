using ShortVideoCutter.Interfaces;
using System.Reflection;

namespace ShortVideoCutter.DI;

public record ServiceData(
    IService Service,
    ServiceLifespan Lifespan,
    Type ServiceType,
    ConstructorInfo Constructor
);
