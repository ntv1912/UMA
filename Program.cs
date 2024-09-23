
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UMA.Context;
using UMA.Services;
using Microsoft.OpenApi.Models;
using UMA.Models;
using System.Security.Claims;
using UMA.Handler;
using UMA.Exceptions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
builder.Services.AddControllers();
var jwtSetting = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnForbidden= context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var exception = new UnauthorizedException();
            var result = new Response(exception.Message);
            result.StatusCode = StatusCodes.Status403Forbidden;
            result.Exception = nameof(UnauthorizedException);
            context.Response.WriteAsJsonAsync(result);
            return Task.CompletedTask;
        },
        OnChallenge=context =>
        {
            context.HandleResponse();
             context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var exception = new UnAuthenticaionException();
            var result = new Response(exception.Message);
            result.StatusCode = StatusCodes.Status401Unauthorized;
            result.Exception= nameof(UnAuthenticaionException);
            context.Response.WriteAsJsonAsync(result);
            return Task.CompletedTask;
        }        
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSetting["Issuer"],
        ValidAudience = jwtSetting["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting["Key"]))
    };
});
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminPolicy",policy=>policy.RequireClaim(claimType:ClaimTypes.Role,allowedValues:((int)Role.Admin).ToString()));
    opt.AddPolicy("UserPolicy", policy => policy.RequireClaim(claimType: ClaimTypes.Role, allowedValues: ((int)Role.User).ToString()));
});
builder.Services.AddSingleton<JwtTokenService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    option =>
    {
        option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authentication",
            Description = "Enter",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme

        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                }
            },new string[] { }
            }
        });
    }
);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(
    options => options.
    WithOrigins("http://localhost:4200")
    .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
app.UseExceptionHandler(option => { });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
