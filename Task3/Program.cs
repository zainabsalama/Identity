using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Task3.Data;
using Task3.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region Database
var connectionString = builder.Configuration.GetConnectionString("Uneversity");
builder.Services.AddDbContext<UneversityContext>(options =>
    options.UseSqlServer(connectionString));
#endregion

#region Identity User
builder.Services.AddIdentity<Employee, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;

    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<UneversityContext>();

#endregion

#region Use Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Cool";
    options.DefaultChallengeScheme = "Cool";
}).AddJwtBearer("Cool", options =>
{
    string keyString = builder.Configuration.GetValue<string>("SecretKey") ?? string.Empty;
    var keyInBytes = Encoding.ASCII.GetBytes(keyString);
    var key = new SymmetricSecurityKey(keyInBytes);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

#endregion

#region Authorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Doctors", policy => policy
        .RequireClaim(ClaimTypes.Role, "Doctor")
        .RequireClaim(ClaimTypes.NameIdentifier));

    options.AddPolicy("Students", policy => policy
        .RequireClaim(ClaimTypes.Role, "Student", "Doctor")
        .RequireClaim("Nationality", "Egyptian")
        .RequireClaim(ClaimTypes.NameIdentifier));
});

#endregion


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
