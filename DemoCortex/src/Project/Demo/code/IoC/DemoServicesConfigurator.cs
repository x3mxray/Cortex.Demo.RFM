using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Demo.Project.Demo.IoC
{
    public class DemoServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvcControllers("Hackathon*");
            serviceCollection.AddApiControllers("Hackathon*");
        }
    }
}