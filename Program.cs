using Microsoft.EntityFrameworkCore;
using AlquilerVehiculos.Data;
using Microsoft.AspNetCore.Authentication.Cookies; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conexión a la base de datos
builder.Services.AddDbContext<DbAlquilerVehiculosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//   CONFIGURAR AUTENTICACIÓN
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";            // página de login
        options.LogoutPath = "/Account/Logout";          // logout
        options.AccessDeniedPath = "/Account/AccessDenied"; // acceso denegado

        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);  // duración de sesión
        options.SlidingExpiration = true;                // refresca la cookie

        options.Cookie.HttpOnly = true;                    // más seguro
        options.Cookie.IsEssential = true;
        

        //Cookie NO persistente ? se borra al cerrar el navegador
        options.Cookie.Expiration = null;
        options.Cookie.MaxAge = null;
    });

builder.Services.AddAuthorization();

var app = builder.Build();


//   CONFIGURACIÓN DEL PIPELINE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//NECESARIO PARA LOGIN/COOKIES
app.UseAuthentication();   
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
