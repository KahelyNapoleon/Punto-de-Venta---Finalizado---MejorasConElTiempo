using Microsoft.EntityFrameworkCore;
using POS.Data;
using System.Globalization;

namespace POS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>
                (options => options.UseSqlServer
                (builder.Configuration.GetConnectionString("Pos")));

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
                options.Cookie.HttpOnly = true; // Asegura que las cookies solo sean accesibles desde el lado del servidor
                options.Cookie.IsEssential = true; // Asegura que la cookie esté siempre disponible
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Dr sgrego para utilizar numeros con comas => Verificar si es necesario []][
            var cultureInfo = new CultureInfo("es-ES");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


            //Se agrego Session
            //builder.Services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
            //    options.Cookie.HttpOnly = true; // Asegura que las cookies solo sean accesibles desde el lado del servidor
            //    options.Cookie.IsEssential = true; // Asegura que la cookie esté siempre disponible
            //});

            //Agrega Session a la app
            app.UseSession();



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
