using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// �������� ������ AniListService
builder.Services.AddScoped<AniListService>();

// �������� ����������� ������� � ����������� � ������
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// �������� ������ �������
builder.Services.AddResponseCompression();



// �������� ����������� � ��������������� (���� �����)
builder.Services.AddControllersWithViews();

// ��������� Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 100;
    options.Limits.MaxRequestBodySize = 10 * 1024;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
});

var app = builder.Build();

// ����������� �������� ���������� ������������ ������ � ������ ����������
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // � ���������� ����������� ���������� ������ � HSTS
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ������������� �� ��� ��������������� HTTP � HTTPS � ��������� ����������� ������
app.UseHttpsRedirection();

// ����������� ����������� ������
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
    }
});

// ��������� ������ �������
app.UseResponseCompression();

// ��������� ����������� �������
app.UseResponseCaching();

// ��������� �������������
app.UseRouting();

// ������������� �� ��� �����������
app.UseAuthorization();

// ��������� ��������� ��� ������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "news",
    pattern: "News/{action=Index}/{id?}",
    defaults: new { controller = "News", action = "Index" });

// ������ ����������
app.Run();
