using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Добавьте сервис AniListService
builder.Services.AddScoped<AniListService>();

// Добавьте контроллеры с представлениями (если нужно)
builder.Services.AddControllersWithViews();

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
app.UseStaticFiles();

// Настройка маршрутизации
app.UseRouting();

// Промежуточное ПО для авторизации
app.UseAuthorization();

// Настройка маршрутов для контроллеров
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");




// Запуск приложения
app.Run();
