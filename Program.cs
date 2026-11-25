using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.data;
using StudentManagementSystem.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

// Automatically apply database migrations and seed default data if needed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        if (!context.Students.Any())
        {
            context.Students.AddRange(
                new Student
                {
                    RollNo = "101",
                    FullName = "Puskar Kumar",
                    Gender = "Male",
                    Email = "kpuskar37@gmail.com",
                    Phone = "09508394115",
                    Course = "B.Tech CSE",
                    Address = "Ghaziabad"
                },
                new Student
                {
                    RollNo = "102",
                    FullName = "Vicky Kumar",
                    Gender = "Male",
                    Email = "kumar@gmail.com",
                    Phone = "09508394115",
                    Course = "B.Tech IT",
                    Address = "Ghaziabad"
                }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
