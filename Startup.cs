using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Npgsql.EntityFrameworkCore.PostgreSQL;


namespace MyApp
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
            services.AddMvc();
            services.AddDbContext<MyAppContext>(options =>
                options.UseSqlServer(
                    Configuration["CONNECTION_STRING"], sqlOptions => sqlOptions.EnableRetryOnFailure())
                // options.UseNpgsql(
                //     GetNpgsqlConnectionString(), sqlOptions => sqlOptions.EnableRetryOnFailure())
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<MyAppContext>();
                context.Database.EnsureCreated();
            }
        }

        private string GetNpgsqlConnectionString()
        {
            var dbUri = new Uri(Configuration["DATABASE_URL"]);
            var userInfoComponents = dbUri.UserInfo.Split(':');

            var userName = userInfoComponents.First();
            var password = userInfoComponents.Last();
            var host = dbUri.Host;
            var database = dbUri.PathAndQuery.TrimStart('/');

            return string.Format("Server={0};Database={1};Username={2};Password={3}", host, database, userName, password);
        }
    }
}
