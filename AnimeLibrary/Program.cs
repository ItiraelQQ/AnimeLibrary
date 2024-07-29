using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Добавьте сервис AniListService
builder.Services.AddScoped<AniListService>();

// Добавьте кэширование ответов и кэширование в памяти
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// Добавьте сжатие ответов
builder.Services.AddResponseCompression();



// Добавьте контроллеры с представлениями (если нужно)
builder.Services.AddControllersWithViews();

// Настройка Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 100;
    options.Limits.MaxRequestBodySize = 10 * 1024;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
});

var app = builder.Build();

// Используйте страницу исключений разработчика только в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // В продакшене используйте обработчик ошибок и HSTS
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Промежуточное ПО для перенаправления HTTP в HTTPS и обработки статических файлов
app.UseHttpsRedirection();

// Кэширование статических файлов
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
    }
});

// Включение сжатия ответов
app.UseResponseCompression();

// Включение кэширования ответов
app.UseResponseCaching();

// Настройка маршрутизации
app.UseRouting();

// Промежуточное ПО для авторизации
app.UseAuthorization();

// Настройка маршрутов для контроллеров
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "news",
    pattern: "News/{action=Index}/{id?}",
    defaults: new { controller = "News", action = "Index" });

// Запуск приложения
app.Run();
