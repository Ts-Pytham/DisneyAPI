using Disney_API.ModelBinder;
using Disney_API.Models;
using Disney_API.Services;
using Microsoft.AspNetCore.Authentication;
using Disney_API.Security;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<DisneyContext>();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
/*
builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuthentication", null);
*/
builder.Services.AddSwaggerGen(options =>
{
        options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
}
);
builder.Services.AddControllers(opt => {
    opt.ModelBinderProviders.Insert(0, new MyCustomBinderProvider());
});

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("SecretKey"));

builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
