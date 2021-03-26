using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Azure.Storage.Blobs;
using BookeryWebApi.Common;
using BookeryWebApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookeryWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton(x => new BlobServiceClient(Configuration.GetConnectionString("BookeryBlobStorage")));
            services.AddSingleton(x => 
                new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>().UseSqlServer(Configuration.GetConnectionString("BookeryDb")).Options));
            services.AddSingleton<IContainerRepository, AzureContainerRepository>();
            services.AddSingleton<IBlobRepository, AzureBlobRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookeryWebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookeryWebApi v1"));
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
