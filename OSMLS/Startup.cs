using System;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetTopologySuite.Geometries;
using OSMLS.Model;
using OSMLS.Services;
using OSMLSGlobalLibrary;

namespace OSMLS
{
	public class Startup
	{
		private const string AllowAllCorsPolicyName = "AllowAll";

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IMapObjectsCollection, MapObjectsCollection>();
			services.AddSingleton<IInheritanceTreeCollection<Geometry>>(provider =>
				provider.GetService<IMapObjectsCollection>()
			);

			var settingsDirectoryPath = $"{AppContext.BaseDirectory}/settings/";

			Directory.CreateDirectory(settingsDirectoryPath);

			services.AddSingleton<IModulesLibrary, ModulesLibrary>();
			services.AddSingleton<IModelService, ModelService>();
			services.AddHostedService(provider => provider.GetService<IModelService>());

			services.AddGrpc();
			services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});

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
			});

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseGrpcWeb();

			app.UseStaticFiles();

			app.UseCors(AllowAllCorsPolicyName);

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<MapService>().EnableGrpcWeb();
				endpoints.MapControllers();
			});
		}
	}
}