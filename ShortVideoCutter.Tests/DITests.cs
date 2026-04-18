using FluentAssertions;
using ShortVideoCutter.DI;
using ShortVideoCutter.Interfaces;

namespace ShortVideoCutter.Tests;

public class DITests
{
    public record A1(A2 A2, A3 A3) : IService;
    public record A2(A3 A3) : IService;
    public record A3() : IService;
    public record A4(A3 A3) : IService;
    public record A5(A4 A4, A2 A2, A3 A3) : IService;
    // 3->4/2->1->5

    [Fact]
    public void RegistrateOrder()
    {
        DIOwner.DI.RegistrateService<A1, A1>();
        DIOwner.DI.RegistrateService<A2, A2>();
        DIOwner.DI.RegistrateService<A3, A3>();
        DIOwner.DI.RegistrateService<A4, A4>();
        DIOwner.DI.RegistrateService<A5, A5>();
        DIOwner.DI.Run();

        var a1 = DIOwner.DI.GetService<A5>();
        var a2 = DIOwner.DI.GetService<A5>();
        var a3 = DIOwner.DI.GetService<A5>();
        var a4 = DIOwner.DI.GetService<A5>();
        var a5 = DIOwner.DI.GetService<A5>();

        a1.Should().NotBeNull();
        a2.Should().NotBeNull();
        a3.Should().NotBeNull();
        a4.Should().NotBeNull();
        a5.Should().NotBeNull();

        a5.A4.A3.Should().NotBeNull();
    }
}
