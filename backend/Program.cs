using System.IdentityModel.Tokens.Jwt;
using System.Text;
using backend.Data;
using backend.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGlobalErrorHandling();
builder.Services.AddDatabase(builder.Configuration);


builder.Services.AddControllers();
builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddJwtAuthentication(builder.Configuration);   


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



var app = builder.Build();

app.MigrateDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRateLimiter();

app.UseGlobalErrorHandling();

app.UseHttpsRedirection();


app.UseCustomCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }