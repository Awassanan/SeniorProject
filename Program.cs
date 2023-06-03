/*
dotnet ef dbcontext scaffold  "server=localhost;port=3306;user=dev;password=12345678;database=SeniorProject"  Pomelo.EntityFrameworkCore.MySql -c SeniorProjectDbContext -o Models -f --no-pluralize
*/

using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = "math",
        ValidAudience = "public",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.SecurityKey))
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

app.UseCors(Options => Options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

partial class Program
{

    public static string SecurityKey = "jkwtQh7Ft7GKzSFo";

    // public static string DomainName = "https://cache111.com";
    public static string DomainName = "https://acadproj1.sc.chula.ac.th";
    public static string UploadPath = "/data/html/upload/seniorproject";
    public static string UploadURL = DomainName + "/upload/seniorproject";
    public static string ContentPath = "/data/html/content/seniorproject";
    public static string ContentURL = DomainName + "/content/seniorproject";
    public static string BackdoorPassword = "12345678";
}