using DemoPRN1.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoPRN1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<PJPRN221Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

            // Add session service
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set thời gian session timeout
                options.Cookie.HttpOnly = true; // Chỉ dùng qua HTTP
                options.Cookie.IsEssential = true; // Đảm bảo session luôn có trong mọi yêu cầu
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Add session middleware
            app.UseSession();

            app.UseAuthorization();

            // Redirect to login page by default
            app.MapGet("/", async context =>
            {
                context.Response.Redirect("/Accounts/Login");
                await Task.CompletedTask;
            });

            app.MapRazorPages();

            app.Run();
        }
    }
}
