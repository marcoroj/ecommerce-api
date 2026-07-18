using ECommerce.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//area servicios
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer("name=DefaultConnection"));

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen();

//area middlewares
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();


app.Run();
