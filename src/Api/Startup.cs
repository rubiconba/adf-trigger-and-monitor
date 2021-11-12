using Api.Filters;
using Api.Options;
using Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.OpenApi.Models;
using Microsoft.Rest;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => {
                options.LowercaseUrls = true;
            });

            var azureConfiguration = Configuration
                .GetSection(AzureOptions.SectionName);

            services.Configure<AzureOptions>(azureConfiguration);

            var azureOptions = azureConfiguration.Get<AzureOptions>();

            var context = new AuthenticationContext($"{azureOptions.ActiveDirectoryAuthority}/{azureOptions.TenantId}");
            var clientCredentials = new ClientCredential(azureOptions.ApplicationId, azureOptions.AuthenticationKey);
            var authResult = context.AcquireTokenAsync(azureOptions.ResourceManagerUrl, clientCredentials).Result;

            ServiceClientCredentials credentials = new TokenCredentials(authResult.AccessToken);

            services.AddTransient(options =>
            {
                return new DataFactoryManagementClient(credentials)
                {
                    SubscriptionId = azureOptions.SubscriptionId
                };
            });

            services.AddTransient<IADFService, ADFService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ADF", Version = "v1" });
            });

            services.AddControllers(i => i.Filters.Add<ErrorFilter>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.EnvironmentName == "Local")
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ADF v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
