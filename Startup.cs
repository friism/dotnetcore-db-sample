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
using Microsoft.Extensions.Logging;

namespace MyApp
{
    public class Startup
    {
        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<Startup>();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private ILogger _logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<MyAppContext>(options => GetDbConnection(options));
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

        private DbContextOptionsBuilder GetDbConnection(DbContextOptionsBuilder options)
        {
            var connectionString = Configuration["CONNECTION_STRING"];
            if (!string.IsNullOrEmpty(connectionString))
            {
                _logger.LogDebug("Found CONNECTION_STRING, using SQL Server");
                return options.UseSqlServer(
                    connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
            }

            var pgDbUrl = Configuration["DATABASE_URL"];
            if (!string.IsNullOrEmpty(pgDbUrl))
            {
                _logger.LogDebug("Found DATABASE_URL, using PostgreSQL");
                return options.UseNpgsql(
                    ParseNpgsqlConnectionString(pgDbUrl), sqlOptions => sqlOptions.EnableRetryOnFailure());
            }

            throw new ArgumentException("No database configuration found");
        }

        private string ParseNpgsqlConnectionString(string databaseUrl)
        {
            var dbUri = new Uri(databaseUrl);
            var userInfoComponents = dbUri.UserInfo.Split(':');

            var userName = userInfoComponents.First();
            var password = userInfoComponents.Last();
            var host = dbUri.Host;
            var database = dbUri.PathAndQuery.TrimStart('/');

            return string.Format("Server={0};Database={1};Username={2};Password={3};SSL Mode=Require", host, database, userName, password);
        }
    }
}
