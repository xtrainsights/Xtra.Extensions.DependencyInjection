using Microsoft.Extensions.DependencyInjection;

namespace Xtra.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to simplify service registration patterns,
/// including factory pattern registration and service bundle support.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a transient service of type <typeparamref name="T"/> and a singleton factory for it.
    /// </summary>
    /// <typeparam name="T">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddFactory<T>(this IServiceCollection services)
        where T : class
    {
        if (services == null) {
            throw new ArgumentNullException(nameof(services));
        }

        return services
            .AddTransient<T>()
            .AddSingleton<Func<T>>(x => x.GetRequiredService<T>);
    }


    /// <summary>
    /// Adds a transient service of type <typeparamref name="TService"/> with an implementation of type <typeparamref name="TImplementation"/> and a singleton factory for it.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddFactory<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (services == null) {
            throw new ArgumentNullException(nameof(services));
        }

        return services
            .AddTransient<TService, TImplementation>()
            .AddSingleton<Func<TService>>(x => x.GetRequiredService<TService>);
    }


    /// <summary>
    /// Adds a singleton factory that returns a service of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service the factory returns.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="factory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="factory"/> is null.</exception>
    public static IServiceCollection AddFactory<T>(this IServiceCollection services, Func<T> factory)
        where T : class
    {
        if (services == null) {
            throw new ArgumentNullException(nameof(services));
        }

        if (factory == null) {
            throw new ArgumentNullException(nameof(factory));
        }

        return services.AddSingleton(factory);
    }


    /// <summary>
    /// Adds a singleton factory that returns a service of type <typeparamref name="T"/>, using the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service the factory returns.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="factory">The factory that creates the service, given an <see cref="IServiceProvider"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="factory"/> is null.</exception>
    public static IServiceCollection AddFactory<T>(this IServiceCollection services, Func<IServiceProvider, T> factory)
        where T : class
    {
        if (services == null) {
            throw new ArgumentNullException(nameof(services));
        }

        if (factory == null) {
            throw new ArgumentNullException(nameof(factory));
        }

        return services.AddSingleton<Func<T>>(sp => () => factory(sp));
    }


    /// <summary>
    /// Adds services from a new instance of <typeparamref name="T"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service bundle to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddServiceBundle<T>(this IServiceCollection services)
        where T : IServiceBundle, new()
    {
        if (services == null) {
            throw new ArgumentNullException(nameof(services));
        }

        return LoadBundle(services, new T());
    }


    /// <summary>
    /// Adds services from the specified <see cref="IServiceBundle"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="bundle">The service bundle to add.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="bundle"/> is null.</exception>
    public static IServiceCollection AddServiceBundle(this IServiceCollection services, IServiceBundle bundle)
    {
        if (services == null) {
            throw new ArgumentNullException(nameof(services));
        }

        if (bundle == null) {
            throw new ArgumentNullException(nameof(bundle));
        }

        return LoadBundle(services, bundle);
    }


    /// <summary>
    /// Adds services from the specified <see cref="IServiceBundle"/> instances to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="bundles">An array of service bundles to add.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="bundles"/> is null.</exception>
    public static IServiceCollection AddServiceBundles(this IServiceCollection services,
        params IServiceBundle[] bundles)
    {
        if (services == null) {
            throw new ArgumentNullException(nameof(services));
        }

        if (bundles == null) {
            throw new ArgumentNullException(nameof(bundles));
        }

        return LoadBundles(services, bundles);
    }


    /// <summary>
    /// Adds services from the specified <see cref="IServiceBundle"/> collection to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="bundles">An enumerable of service bundles to add.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="bundles"/> is null.</exception>
    public static IServiceCollection AddServiceBundles(this IServiceCollection services,
        IEnumerable<IServiceBundle> bundles)
    {
        if (services == null) {
            throw new ArgumentNullException(nameof(services));
        }

        if (bundles == null) {
            throw new ArgumentNullException(nameof(bundles));
        }

        return LoadBundles(services, bundles);
    }


    private static IServiceCollection LoadBundle<T>(IServiceCollection services, T bundle)
        where T : IServiceBundle
    {
        bundle.Load(services);
        return services;
    }


    private static IServiceCollection LoadBundles(IServiceCollection services, IEnumerable<IServiceBundle> bundles)
    {
        foreach (var bundle in bundles) {
            bundle.Load(services);
        }

        return services;
    }
}