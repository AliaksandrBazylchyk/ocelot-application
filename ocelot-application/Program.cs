using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configurations loader
IConfiguration ocelotConfiguration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile($"ocelot-securedApi.json", true, true)
    .AddJsonFile($"ocelot-unsecuredApi.json", true, true)
    .Build();

// Services loader
builder.Services.AddRouting();
builder.Services.AddOcelot(ocelotConfiguration);
builder.Services.AddAuthentication(s =>
{
    s.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    s.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "http://ocelot-identityserver:80";
    /*********************************************************/
    /*              Comment this code on Release             */
    /*********************************************************/
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;
    options.TokenValidationParameters.ValidateIssuer = false;
    /*********************************************************/
});
builder.Services.AddCors();

// App initializer
var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Cors configuration
app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

await app.UseOcelot();

app.Run();