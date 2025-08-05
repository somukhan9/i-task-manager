using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Policies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Add DbContext
builder.AddTaskManagerDbContext();

// DI through extension method
builder.AddSqlConnectionFactory();
builder.AddServices();

// Add Background Services
//builder.AddHostedBackgroundServices();
builder.AddBackgroundServiceFactoryManager();


builder.AddIdentity();
builder.AddApplicationCookie();
// builder.AddJwtBearer();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Sd.TaskOwnerPolicy, policy => policy.Requirements.Add(new TaskOwnerRequirement()));
});

builder.Services.AddRouting(options => { options.LowercaseUrls = true; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDatabaseMigrateMiddleware();
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();