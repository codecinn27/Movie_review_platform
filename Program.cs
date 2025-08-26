using Microsoft.EntityFrameworkCore;
using MovieAPI.AppDataContext;
using MovieAPI.Middleware;
using MovieAPI.Interface;
using MovieAPI.Services;
using MovieAPI.MappingProfiles;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); // Add this line
builder.Services.AddProblemDetails();  
builder.Services.AddLogging();


builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetSection("DbSettings:ConnectionString").Value,
        ServerVersion.AutoDetect(builder.Configuration.GetSection("DbSettings:ConnectionString").Value)
    )
);
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IReviewService, ReviewService>();


// JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    
    options.Events = new JwtBearerEvents
    {
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(
                "{\"error\": \"Access denied. Only admin is allowed to use this route.\"}"
            );
        },
        OnChallenge = context =>
        {
            context.HandleResponse(); // Prevent default behavior
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(
                "{\"error\": \"You must provide a valid JWT token.\"}"
            );
        }
    };
});



var app = builder.Build();

{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider; 
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseRouting();  
app.UseAuthorization();
app.MapControllers();

app.Run();