using CSharpClicker.Domain;
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

            //app.MapControllers();

            app.UseMvc();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapGet("/", () => "Hello World!");
            app.MapHealthChecks("health-check");

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddSwaggerGen();
            services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));
            services.AddAuthentication();
            services.AddAuthorization();
            services.AddMvcCore(o => o.EnableEndpointRouting = false)
                .AddApiExplorer();

            IdentityInitializer.AddIdentity(services);
            DbContextInitializer.AddAppDbContext(services);
        }
    }
}