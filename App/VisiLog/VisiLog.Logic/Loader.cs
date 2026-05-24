using CodeFactory.NDF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VisiLog.Logic.Contracts;

namespace VisiLog.Logic
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
            //Intentionally blank, this is for child libraries to override and load their own dependency injection.
        }

        /// <summary>
        /// Loads dependency injections that are setup and configured manually.
        /// </summary>
        /// <param name="serviceCollection">The dependency injection provider to register services with.</param>
        /// <param name="configuration">The source configuration to provide for dependency injection.</param>
        protected override void LoadManualRegistration(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<ILogSourceLogic, LogSourceLogic>();
            serviceCollection.AddScoped<ILogMessageLogic, LogMessageLogic>();
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
