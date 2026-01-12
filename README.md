# Xtra.Extensions.DependencyInjection

A set of extension methods for `IServiceCollection` to simplify service registration patterns in .NET applications, featuring factory registration and service bundles.

## Features

- **Factory Pattern Registration**: Easily register `Func<T>` factories that resolve services from the DI container.
- **Service Bundles**: Group related service registrations into reusable modules using the `IServiceBundle` interface.
- **Fluent API**: Maintains the standard `IServiceCollection` fluent interface.
- **Modern .NET Support**: Targets `netstandard2.1` and tested against .NET 8, 9, and 10.

## Installation

Install via NuGet:

```bash
dotnet add package Xtra.Extensions.DependencyInjection
```

## Usage

### Factory Registration

The `AddFactory` methods allow you to register a service along with a `Func<T>` that can be used to create instances of that service.

#### 1. Simple Factory
Registers `MyService` as transient and a `Func<MyService>` that resolves it.

```csharp
services.AddFactory<MyService>();

// Usage in a consumer
public class Consumer(Func<MyService> factory)
{
    public void DoWork()
    {
        var service = factory(); // Resolves a new instance
    }
}
```

#### 2. Interface with Implementation Factory
Registers `IMyService` as transient using `MyService` implementation, and a `Func<IMyService>`.

```csharp
services.AddFactory<IMyService, MyService>();
```

#### 3. Custom Factory Delegate
Registers a singleton factory delegate.

```csharp
services.AddFactory<IMyService>(() => new MyService("custom configuration"));
```

#### 4. Factory with IServiceProvider Access
Registers a factory that uses `IServiceProvider` to resolve dependencies.

```csharp
services.AddFactory<IMyService>(sp => 
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new MyService(config["Setting"]);
});
```

### Service Bundles

Service bundles allow you to encapsulate multiple related registrations into a single class. They're inspired by Autofac's modules,
and provide a way to organize and reuse service registrations.

#### Define a Bundle

```csharp
public class DataAccessBundle : IServiceBundle
{
    public void Load(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
    }
}
```

#### Register a Bundle

You can register bundles by type, instance, or multiple instances at once.

```csharp
// By type (requires parameterless constructor)
services.AddServiceBundle<DataAccessBundle>();

// By instance
services.AddServiceBundle(new MessagingBundle("ConnectionString"));

// Multiple bundles
services.AddServiceBundles(new FooBundle(), new BarBundle());

// From a collection
var bundles = new List<IServiceBundle> { new CoreBundle(), new WebBundle() };
services.AddServiceBundles(bundles);
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.