using Microsoft.Extensions.DependencyInjection;

namespace Xtra.Extensions.DependencyInjection.Tests;

public class ServiceCollectionTests
{
    [Fact]
    public void ServiceCollection_AddFactory_OfImplementation()
    {
        var sc = new ServiceCollection();
        sc.AddFactory<FooService>();

        using var sp = sc.BuildServiceProvider();
        var factory = sp.GetRequiredService<Func<FooService>>();

        Assert.IsType<FooService>(factory());
        Assert.NotSame(factory(), factory());
    }


    [Fact]
    public void ServiceCollection_AddFactory_OfInterface()
    {
        var sc = new ServiceCollection();
        sc.AddFactory<ITestService, FooService>();

        using var sp = sc.BuildServiceProvider();
        var factory = sp.GetRequiredService<Func<ITestService>>();

        Assert.IsType<FooService>(factory());
        Assert.NotSame(factory(), factory());
    }


    [Fact]
    public void ServiceCollection_AddFactory_OfDelegate()
    {
        var sc = new ServiceCollection();
        sc.AddFactory<ITestService>(() => new FooService());

        using var sp = sc.BuildServiceProvider();
        var factory = sp.GetRequiredService<Func<ITestService>>();

        Assert.IsType<FooService>(factory());
        Assert.NotSame(factory(), factory());
    }


    [Fact]
    public void ServiceCollection_AddFactory_OfResolvingDelegate()
    {
        var sc = new ServiceCollection();
        sc.AddTransient<BarService>();
        sc.AddFactory<ITestService>(
            sp => new FooService {
                Wrapped = sp.GetRequiredService<BarService>()
            }
        );

        using var sp = sc.BuildServiceProvider();
        var factory = sp.GetRequiredService<Func<ITestService>>();

        Assert.IsType<FooService>(factory());
        Assert.NotSame(factory(), factory());
        Assert.NotSame(factory().Wrapped, factory().Wrapped);
    }


    [Fact]
    public void ServiceCollection_AddFactory_NullCheck()
    {
        var sc = new ServiceCollection();
       
        // Test services parameter is null
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddFactory<FooService>());
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddFactory<ITestService, FooService>());
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddFactory<FooService>(() => new FooService()));
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddFactory<FooService>(sp => new FooService()));

        // Test other parameters are null
        Assert.Throws<ArgumentNullException>(() => sc.AddFactory<FooService>((Func<FooService>)null!));
        Assert.Throws<ArgumentNullException>(() => sc.AddFactory<FooService>((Func<IServiceProvider, FooService>)null!));
    }


    [Fact]
    public void ServiceCollection_AddServiceBundle_WithGenerics()
    {
        var sc = new ServiceCollection();
        sc.AddServiceBundle<FooBundle>();

        using var sp = sc.BuildServiceProvider();
        Assert.IsType<FooService>(sp.GetRequiredService<FooService>());
    }


    [Fact]
    public void ServiceCollection_AddServiceBundle_WithInstance()
    {
        var sc = new ServiceCollection();
        sc.AddServiceBundle(new FooBundle());

        using var sp = sc.BuildServiceProvider();
        Assert.IsType<FooService>(sp.GetRequiredService<FooService>());
    }


    [Fact]
    public void ServiceCollection_AddServiceBundles_WithParams()
    {
        var sc = new ServiceCollection();
        sc.AddServiceBundles(new FooBundle(), new BarBundle());

        using var sp = sc.BuildServiceProvider();
        Assert.IsType<FooService>(sp.GetRequiredService<FooService>());
        Assert.IsType<BarService>(sp.GetRequiredService<BarService>());
    }


    [Fact]
    public void ServiceCollection_AddServiceBundles_WithList()
    {
        var sc = new ServiceCollection();
        sc.AddServiceBundles(new List<IServiceBundle> { new FooBundle(), new BarBundle() });

        using var sp = sc.BuildServiceProvider();
        Assert.IsType<FooService>(sp.GetRequiredService<FooService>());
        Assert.IsType<BarService>(sp.GetRequiredService<BarService>());
    }


    [Fact]
    public void ServiceCollection_AddServiceBundles_NullCheck()
    {
        var sc = new ServiceCollection();

        // Test services parameter is null
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddServiceBundle<FooBundle>());
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddServiceBundle(new FooBundle()));
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddServiceBundles(new FooBundle()));
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddServiceBundles(new List<IServiceBundle> { new FooBundle() }));

        // Test other parameters are null
        Assert.Throws<ArgumentNullException>(() => sc.AddServiceBundle((IServiceBundle)null!));
        Assert.Throws<ArgumentNullException>(() => sc.AddServiceBundles((IServiceBundle[])null!));
        Assert.Throws<ArgumentNullException>(() => sc.AddServiceBundles((IEnumerable<IServiceBundle>)null!));
    }


    private class FooBundle : IServiceBundle
    {
        public void Load(IServiceCollection services) => services.AddTransient<FooService>();
    }


    private class BarBundle : IServiceBundle
    {
        public void Load(IServiceCollection services) => services.AddTransient<BarService>();
    }


    private interface ITestService
    {
        ITestService? Wrapped { get; set; }
    }


    private class FooService : ITestService
    {
        public ITestService? Wrapped { get; set; }
    }


    private class BarService : ITestService
    {
        public ITestService? Wrapped { get; set; }
    }
}