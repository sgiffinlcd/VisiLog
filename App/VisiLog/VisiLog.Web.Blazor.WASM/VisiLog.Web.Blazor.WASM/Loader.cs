using CodeFactory.NDF;

namespace VisiLog.Web.Blazor.WASM
{
    /// <summary>
    /// Dependency injection loader for the library. This is used to load dependency injection for the library and any child libraries that are subscribed to by this library.
    /// </summary>
    public class Loader : DependencyInjectionLoader
    {


        /// <summary>
        /// Loads child libraries that are subscribed to by this library.
        /// </summary>
        /// <param name="serviceCollection">The dependency injection provider to register services with.</param>
        /// <param name="configuration">The source configuration to provide for dependency injection.</param>
        protected override void LoadLibraries(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var sqlDataLoader = new Data.SQL.Loader();
            var logicLoader = new Logic.Loader();
            var serviceWebAPILoader = new Service.WebAPI.Loader();

            //Load child library dependency injection. Leaf to trunk load order.
            sqlDataLoader.Load(serviceCollection, configuration);
            logicLoader.Load(serviceCollection, configuration);
            serviceWebAPILoader.Load(serviceCollection, configuration);

        }

        /// <summary>
        /// Loads dependency injections that are setup and configured manually.
        /// </summary>
        /// <param name="serviceCollection">The dependency injection provider to register services with.</param>
        /// <param name="configuration">The source configuration to provide for dependency injection.</param>
        protected override void LoadManualRegistration(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            //Intentionally blank, this is for child libraries to override and load their own manual dependency injection.
        }

        /// <summary>
        /// Loads dependency injection registrations.
        /// </summary>
        /// <param name="serviceCollection">The dependency injection provider to register services with.</param>
        /// <param name="configuration">The source configuration to provide for dependency injection.</param>
        protected override void LoadRegistration(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            //Intentionally blank, this is for child libraries to override and load their own dependency injection.
        }
    }
}
