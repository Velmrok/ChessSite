using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
    )
);

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
var app = builder.Build();




app.UseHttpsRedirection();

app.MapControllers();


app.Run();

