using backend.Extensions;
using backend.Options;
using backend.Policies;
using backend.Services;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGlobalErrorHandling();
builder.Services.AddDatabase(builder.Configuration, builder.Environment.IsEnvironment("Test"));


builder.Services.AddControllers();
builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuthentication(builder.Configuration);   

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OwnerOrAdmin", policy =>
        policy.Requirements.Add(new OwnerOrAdminRequirement()));
});
builder.Services.AddSingleton<IAuthorizationHandler, OwnerOrAdminHandler>();

var isTestEnvironment = builder.Configuration.GetValue<bool>("IsTestEnvironment");

if (!isTestEnvironment)
{
    builder.Services.AddCustomCache(builder.Configuration);
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddCustomModelValidation();

builder.Services.AddCustomRateLimiting();

builder.Services.AddCustomCors();

var storageProvider = builder.Configuration["Storage:Provider"] ?? "Local";

if (storageProvider == "R2")
    builder.Services.AddSingleton<IStorageService, R2StorageService>();
else
    builder.Services.AddSingleton<IStorageService, LocalStorageService>();

builder.Services.Configure<StorageOptions>(
    builder.Configuration.GetSection("Storage"));

builder.Services.AddSignalR();


var app = builder.Build();

app.MigrateDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
     app.UseStaticFiles();
}
app.UseRateLimiter();

app.UseGlobalErrorHandling();

app.UseHttpsRedirection();


app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCustomCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.MapHub<backend.Hubs.MainHub>("/mainhub");
app.Run();

public partial class Program { }