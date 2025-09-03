using Microsoft.Data.SqlClient;
using RTSAct2015.Data.Interfaces;
using RTSAct2015.Data.Repositories;
using RTSAct2015.Services;
using RTSAct2015Services.Data.Repositories;
using RTSAct2015Services.Interfaces.IRepository;
using RTSAct2015Services.Interfaces.IServices;
using RTSAct2015Services.Services;
using RTSAct2015Services.Services.Implementation;
using RTSAct2015Services.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization();

// Register repositories
builder.Services.AddScoped<IGatturComplaintRepository, GatturComplaintRepository>();
builder.Services.AddScoped<ITreeTrimmingRepository, TreeTrimmingRepository>();
builder.Services.AddScoped<IOFCPermissionRepository, OFCPermissionRepository>();
builder.Services.AddScoped<IPotholeComplaintRepository, PotholeComplaintRepository>();
builder.Services.AddScoped<ITreeFellingRepository, TreeFellingRepository>();
builder.Services.AddScoped<IDepositRefundRepository, DepositRefundRepository>();
builder.Services.AddScoped<ITrackApplicationRepository, TrackApplicationRepository>();

// Register services
builder.Services.AddScoped<IGatturComplaintService, GatturComplaintService>();
builder.Services.AddScoped<ITreeTrimmingService, TreeTrimmingService>();
builder.Services.AddScoped<IOFCPermissionService, OFCPermissionService>();
builder.Services.AddScoped<IPotholeComplaintService, PotholeComplaintService>();
builder.Services.AddScoped<ITreeFellingService, TreeFellingService>();
builder.Services.AddScoped<IDepositRefundService, DepositRefundService>();
builder.Services.AddScoped<ITrackApplicationService, TrackApplicationService>();

// Add these to your Program.cs
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IBirthCertificateRepository, BirthCertificateRepository>();
builder.Services.AddScoped<IBirthCertificateService, BirthCertificateService>();
builder.Services.AddScoped<IDeathCertificateRepository, DeathCertificateRepository>();
builder.Services.AddScoped<IDeathCertificateService, DeathCertificateService>();
builder.Services.AddScoped<IMarriageCertificateRepository, MarriageCertificateRepository>();
builder.Services.AddScoped<IMarriageCertificateService, MarriageCertificateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started in {env} mode.", app.Environment.EnvironmentName);

// Database connection test on startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var cs = config.GetConnectionString("DefaultConnection");
        using var conn = new SqlConnection(cs);
        conn.Open();
        logger.LogInformation("✓ Database connection successful.");
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "✗ Database connection failed on startup.");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
