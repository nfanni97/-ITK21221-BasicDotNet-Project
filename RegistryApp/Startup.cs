using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using RegistryApp.Models;
using Microsoft.EntityFrameworkCore;
using RegistryApp.Services.Interfaces;
using RegistryApp.Services.Implementations;
using RegistryApp.Models.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Cryptography;

namespace RegistryApp
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RegistryApp", Version = "v1" });
            });
            services.AddAutoMapper(
                AppDomain.CurrentDomain.GetAssemblies()
            );
            services.AddDbContext<RegistryContext>(opt => 
                opt.UseSqlServer(Configuration.GetConnectionString("Registry")));
            //TODO: scoped services
            services.AddScoped<ICategoryService,CategoryService>();
            services.AddScoped<IProductService,ProductService>();
            services.AddScoped<IRegistryService,RegistryService>();
            //authentication
            var key = new HMACSHA256(Encoding.UTF8.GetBytes(Configuration["JWT:secret"]));
            services
                .AddIdentity<ApplicationUser,IdentityRole>()
                .AddEntityFrameworkStores<RegistryContext>()
                .AddDefaultTokenProviders();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                //  Add JWt bearer
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidAudience = Configuration["JWT:validAudience"],
                        ValidIssuer = Configuration["JWT:validIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:secret"])),
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Registry App v1"));
            } else {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
