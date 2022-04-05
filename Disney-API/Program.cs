using Disney_API.ModelBinder;
using Disney_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<DisneyContext>();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
    {
        options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    }
);
builder.Services.AddControllers(opt => {
    opt.ModelBinderProviders.Insert(0, new MyCustomBinderProvider());
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
