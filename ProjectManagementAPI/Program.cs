using ProjectManagementAPI.GraphQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new ProjectManagementAPI.JsonSnakeCaseNamingPolicy();
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ProjectManagementAPI.Services.IEmailService, ProjectManagementAPI.Services.EmailService>();



/* ================= CORS ================= */

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                // Allow localhost and any vercel.app domain
                return origin.StartsWith("http://localhost") ||
                       origin.StartsWith("https://localhost") ||
                       origin.EndsWith(".vercel.app");
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



/* ================= JWT ================= */

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.TokenValidationParameters =
            new TokenValidationParameters
            {

                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,


                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],


                ValidAudience =
                    builder.Configuration["Jwt:Audience"],


                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    )

            };

    });



builder.Services.AddAuthorization();



/* ================= GRAPHQL ================= */

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();



var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}



// app.UseHttpsRedirection(); // Disabled for local mobile testing over HTTP



app.UseCors("AllowFrontend");



/* IMPORTANT ORDER */

app.UseAuthentication();

app.UseAuthorization();



app.MapControllers();

app.MapGraphQL();



app.Run();