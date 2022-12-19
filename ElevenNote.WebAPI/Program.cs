using ElevenNote.Data;
using ElevenNote.Services.User;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ElevenNote.Services.Token;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//^^got this from a mix of the module and what Terry gave me (below) - the code Terry had me use gave me an error when it came to Postman; something about the token not being a string or something? Whatever. Mixed them and now it's working.

// builder.Services.AddAuthentication(opt =>
// {
//     opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //Bearer "Magic String"
//     opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(opt =>  //certian parameters in which the bearer authentication is going to work
// {
//     opt.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true, //if our API didn't create the token then its REJECTED
//         ValidateIssuer = true,  //things came from US the API
//         ValidateAudience = true, //things came from someone who WE the API RECOGNIZED;
//         ValidateLifetime = true,  //make sure that the token will only last for so long.
//         ClockSkew = TimeSpan.Zero, //Sets the clock for the validation lifetime
//         ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//         ValidAudience = builder.Configuration["JwtSettings:Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
//     };
// });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
//<<this trickles down to parent object's constructor, then use sqlserver
{
    options.UseSqlServer(connectionString);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); //make sure you are who you say you are - that you're supposed to be here

app.UseAuthorization(); //give you permissions based on who you are once that's been verified by your credentials

app.MapControllers();

app.Run();
