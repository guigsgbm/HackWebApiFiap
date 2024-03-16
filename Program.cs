using Azure.Storage.Blobs;
using HackWebApi.Messaging;
using Infrastructure.DB;
using Infrastructure.DB.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ReqRepository>();
var connection = builder.Configuration.GetConnectionString("AzureDB");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connection));

builder.Services.Configure<ItemMessagingConfig>(builder.Configuration.GetSection("AzureSB"));
builder.Services.AddScoped<ItemMessaging>();

builder.Services.AddScoped(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("Blob");
    return new BlobServiceClient(connectionString);
});

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().WithHeaders("Content-Type", "Authorization");
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");

app.Use((context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return Task.CompletedTask;
    }
    return next();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
