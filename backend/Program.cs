using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGlobalErrorHandling();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
    )
);

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
var app = builder.Build();

app.UseGlobalErrorHandling();

app.UseHttpsRedirection();

app.MapControllers();


app.Run();

public partial class Program { }