using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OSMLS.Model;
using OSMLS.Services;

namespace OSMLS
{
	public class Startup
	{
		private const string AllowAllCorsPolicyName = "AllowAll";

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<MapObjectsCollection>();

			var settingsDirectoryPath = $"{AppContext.BaseDirectory}/settings/";
			var modulesDirectoryPath = $"{AppContext.BaseDirectory}/modules/";
			var osmFilePath = $"{AppContext.BaseDirectory}/map.osm";

			Directory.CreateDirectory(settingsDirectoryPath);
			Directory.CreateDirectory(modulesDirectoryPath);

			services.AddSingleton(provider => new ModulesLibrary(
				modulesDirectoryPath,
				osmFilePath,
				provider.GetService<MapObjectsCollection>()
			));
			services.AddSingleton<ModelService>();
			services.AddHostedService(provider => provider.GetService<ModelService>());

			services.AddGrpc();
			services.AddControllers();

			services.AddSwaggerGen();

			services.AddCors(options => options.AddPolicy(AllowAllCorsPolicyName, builder =>
			{
				builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
			}));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseSwagger();

			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "OSMLS API V1");
				options.RoutePrefix = string.Empty;
			});

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseGrpcWeb();

			app.UseCors();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<MapService>().EnableGrpcWeb().RequireCors(AllowAllCorsPolicyName);
				endpoints.MapControllers();
			});
		}
	}
}