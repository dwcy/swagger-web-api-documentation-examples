using ITHS.Webapi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        //Add auth
        builder.Services.AddAuthentication(x => {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true
                    };
                });

        var config = builder.Configuration;

        ConfigureSwagger(builder.Services);

        builder.Services.AddResponseCompression();
        var app = builder.Build();
        app = ConfigureAppBuilder(app);
        app.Run();


        IServiceCollection ConfigureSwagger(IServiceCollection service)
        {
            service.AddEndpointsApiExplorer();

            service.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
            });
            service.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                        new OpenApiInfo
                        {
                            Title = "ITHS - V1",
                            Version = "v1",
                            Description = "Here we go v1 of the api",
                            TermsOfService = new Uri("http://toSomewhere.com"),
                            Contact = new OpenApiContact
                            {
                                Name = "Please don't contact me",
                                Email = "Nope@tempuri.org"
                            },
                            License = new OpenApiLicense
                            {
                                Name = "Apache 2.0",
                                Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
                            }
                        });
                options.SwaggerDoc("v2", new OpenApiInfo { Title = "ITHS Demo", Description = "Here we go v2 of the api now even better!!!", Version = "v2" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {{
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

                //xml comments
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            return service;
        }

        WebApplication ConfigureAppBuilder(WebApplication app)
        {
            app.UseResponseCompression();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger(c =>
                {
                    //c.RouteTemplate = "api-docs/{documentName}/swagger.json";
                });
                app.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "swagger";
                    options.SwaggerEndpoint("v1/swagger.json", "V1");
                    options.SwaggerEndpoint("v2/swagger.json", "V2");

                    //custom css and html
                    //options.InjectStylesheet("/swagger-ui/custom.css");
                    //options.IndexStream = () => GetType().Assembly
                    //    .GetManifestResourceStream("ITHS.Webapi.Swagger.index.html");

                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}