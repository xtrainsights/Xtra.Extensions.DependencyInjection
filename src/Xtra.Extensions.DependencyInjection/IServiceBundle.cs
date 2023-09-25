using Microsoft.Extensions.DependencyInjection;

namespace Xtra.Extensions.DependencyInjection;

public interface IServiceBundle
{
    void Load(IServiceCollection services);
}