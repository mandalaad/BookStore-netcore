using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

public class Program {
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        //public IConfiguration Configuration { get; }

        //public void ConfigureServices(IServiceCollection services)
        //{

        //}

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddCors(o => {
            o.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        });
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Book Store API",
                Version = "v1",
                Description = "This is an educational API for a Book Store"
            });
            var xfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xpath = Path.Combine(AppContext.BaseDirectory, xfile);
            c.IncludeXmlComments(xpath);
        });

        builder.Services.AddSingleton<ILoggerService, LoggerService>();

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Store API");
            c.RoutePrefix = "";
        });

        app.UseHttpsRedirection();
        //app.UseStaticFiles();

        app.UseCors("CorsPolicy");
        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.UseAuthentication();

        //app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        app.MapControllers();
        app.Run();

    }
}
