using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataStorageService.Endpoints.DataStorage.AggregateData;
using StructureMap;
using DataStorageService.AppSettings;
using Microsoft.Data.Sqlite;
using System.IO;
using DataStorageService.Endpoints.DataStorage.DatabaseInterfaces;

namespace DataStorageService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ConfigureDatabase(services);
            services.AddMvc().AddControllersAsServices();
            return ConfigureIoC(services);
        }

        private void ConfigureDatabase(IServiceCollection services) {
            var dependencyContainer = GetIoCContainer(services);

            var applicationSettings = dependencyContainer.GetInstance<IApplicationSettings>();
            var aggregateDataFileLocation = Path.Combine(applicationSettings.SqliteStorageFolderLocation, applicationSettings.AggregateSqliteFileName);
            Directory.CreateDirectory(applicationSettings.SqliteStorageFolderLocation);
            File.Create(aggregateDataFileLocation);
            services.AddDbContext<AggregateDataContext>(ops => ops.UseSqlite(AggregateDataContext.GetSqliteString(aggregateDataFileLocation)));
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

        }
        public IServiceProvider ConfigureIoC(IServiceCollection services)
        {
            var container = GetIoCContainer(services);
            return container.GetInstance<IServiceProvider>();
        }

        private Container GetIoCContainer(IServiceCollection services) {

            var container = new Container();
            container.Configure(config =>
            {
                // Register stuff in container, using the StructureMap APIs...
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.WithDefaultConventions();
                    
                });
                config.For<IImportedDataPointRepository>().Use<SqliteImportedDataPointRepository>()
                    .Ctor<IApplicationSettings>().IsTheDefault();
                //Populate the container using the service collection

                config.Populate(services);
            });
            return container;
        }
    }
}
