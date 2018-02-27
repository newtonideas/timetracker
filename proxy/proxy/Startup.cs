using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using proxy.Services;
using Microsoft.EntityFrameworkCore;
using proxy.Data;
using proxy.AuthServices;

namespace proxy
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

            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddMvc();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddScoped<IProjectRepository, ExtranetProjectsRepository>();
            services.AddScoped<ITaskRepository, ExtranetTasksRepository>();
            services.AddScoped<ITimelogRepository, ExtranetTimelogsRepository>();
            services.AddScoped<IUserRepository, ExtranetUsersRepository>();

            services.AddScoped<IAuthService, ExtranetAuthService>();

            services.AddScoped<ITokenStorage, DbTokenStorage>();

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
    }
}
