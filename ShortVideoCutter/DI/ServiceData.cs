using ShortVideoCutter.Interfaces.ModuleInterfaces;
using System.Reflection;

namespace ShortVideoCutter.DI;

public record ServiceData(
    IService Service,
    EServiceLifespan Lifespan,
    Type ServiceType,
    ConstructorInfo Constructor
);
