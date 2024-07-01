using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// �������� ������ AniListService
builder.Services.AddScoped<AniListService>();

// �������� ����������� � ��������������� (���� �����)
builder.Services.AddControllersWithViews();

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
app.UseStaticFiles();

// ��������� �������������
app.UseRouting();

// ������������� �� ��� �����������
app.UseAuthorization();

// ��������� ��������� ��� ������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");




// ������ ����������
app.Run();
