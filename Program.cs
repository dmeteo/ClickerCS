using CSharpClicker.Domain;
using CSharpClicker.Infrastructure.Abstractions;
using CSharpClicker.Infrastructure.DataAccess;
using CSharpClicker.Initializers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CSharpClicker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services);

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            using var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            DbContextInitializer.InitializeDbContext(appDbContext);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();
            app.MapDefaultControllerRoute();
            app.MapHealthChecks("health-check");

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddSwaggerGen();

            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));

            services.AddAuthentication();
            services.AddAuthorization();
            services.AddControllersWithViews();

            services.AddScoped<IAppDbContext, AppDbContext>();

            IdentityInitializer.AddIdentity(services);
            DbContextInitializer.AddAppDbContext(services);
        }
    }
}