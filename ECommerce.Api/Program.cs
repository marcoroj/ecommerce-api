using ECommerce.Api.Data;
using ECommerce.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//area servicios
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer("name=DefaultConnection"));

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();

var corsConfiguration = "ECommerceApiCors";

builder.Services.AddCors(config =>
{
    config.AddPolicy(corsConfiguration, policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader().WithExposedHeaders(new string[] { "X-Total-Count" });
        policy.AllowAnyMethod();
    });
});

//area middlewares
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseCors(corsConfiguration);

app.MapControllers();


app.Run();
