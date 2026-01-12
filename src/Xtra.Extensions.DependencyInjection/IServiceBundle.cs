using Microsoft.Extensions.DependencyInjection;

namespace Xtra.Extensions.DependencyInjection;

/// <summary>
/// Defines a bundle of services that can be loaded into an <see cref="IServiceCollection"/>.
/// </summary>
public interface IServiceBundle
{
    /// <summary>
    /// Loads the services in this bundle into the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    void Load(IServiceCollection services);
}