using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XxlJobExecutorSharp;

namespace XxlJobExecutorWeb
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddXxlJob(options =>
            {
                options.AdminUrl = "http://localhost:8080/xxl-job-admin";
                options.Token = "LDgVTSL2m3oEZMvgMAtJzEhhD8rT0bRpQXQ8583E";
                options.HeartbeatIntervalSecond = 10;
                options.ExecutorAppName = "test";
                options.ExecutorUrl = "http://localhost:19187/api/xxljob/";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseXxlJob();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
