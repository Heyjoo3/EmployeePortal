using AutoMapper;
using EmployeePortal.Api.MappingProfiles;
using EmployeePortal.Core.Data;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;
using EmployeePortal.Data;
using EmployeePortal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace EmployeePortal
{
    public class Startup
    {
        private IServiceCollection _services;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _services = services;
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddIdentity<Employee, IdentityRole>()
                    .AddEntityFrameworkStores<EmployeePortalContext>()
                    .AddDefaultTokenProviders();
            services.AddDbContext<EmployeePortalContext>(opt =>
              opt.UseSqlServer(Configuration.GetConnectionString("EmployeePortal"), b => b.MigrationsAssembly("EmployeePortal.Core")),
              ServiceLifetime.Scoped);
            services.AddSingleton((Serilog.ILogger)Log.Logger);
            services.AddScoped<AbsenceDto>();
            services.AddScoped<CredentialsDto>();
            services.AddScoped<EmployeeDto>();
            services.AddScoped<EmployeeUpdateDto>();
            services.AddScoped<EmployeeAuthenticationDto>();
            services.AddScoped<GetEmployeeDto>();
            services.AddScoped<PublicHolidayDto>();
            services.AddScoped<RequestDto>();
            services.AddScoped<RequestFormDto>();
            services.AddScoped<SideNavDto>();
            services.AddScoped<SideNavEmployeeDto>();
            services.AddScoped<VacationDto>();

            services.AddScoped<IAbsenceService, AbsenceService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IPublicHolidayService, PublicHolidayService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISideNavService, SideNavService>();
            //services.AddScoped<ISmtpMailService, SmtpMailService>();
            services.AddScoped<IUpdateVacationStatusService, UpdateVacationStatusService>();
            services.AddScoped<IVacationService, VacationService>();
            services.AddScoped<IVacationStatusEmailService, VacationStatusEmailService>();
            services.AddScoped<IOnboardingService, OnboardingService>();
         
            services.AddScoped<SmtpMailService>();
            services.AddScoped<IcsEventService>();

            services.AddScoped<IAbsenceRepository, AbsenceRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IPublicHolidayRepository, PublicHolidayRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IVacationRepository, VacationRepository>();
            services.AddScoped<IOnboardingRepository, OnboardingRepository>();






            services.AddHttpContextAccessor();
            services.AddEndpointsApiExplorer();
            services.AddAutoMapper(typeof(EmployeeProfile));
            services.AddAutoMapper(typeof(VacationProfile));
            services.AddAutoMapper(typeof(AbsenceProfile));
            services.AddAutoMapper(typeof(OnboardingProfile));
            services.AddCors(c =>
            {
                c.AddPolicy("CorsPolicy", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowSpecificOrigin",
            //        builder => builder.WithOrigins("http://localhost:5173")
            //                          .AllowAnyOrigin()
            //                          .AllowAnyHeader()
            //                          .AllowAnyMethod());
            //});

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "PortalApp";
            });
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "EmployeePortalAPI", Version = "v1" });
                option.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                option.EnableAnnotations();
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                            "Example: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
          .AddJwtBearer(x =>
          {
              x.RequireHttpsMetadata = false;
              x.IncludeErrorDetails = true;
              x.SaveToken = true;
              x.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  ValidateIssuer = false,
                  ValidateAudience = false,
                  ValidateLifetime = true,
                  ClockSkew = TimeSpan.Zero,
                  IssuerSigningKey = new SymmetricSecurityKey(key)
              };
          });

            services.AddControllers();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceScopeFactory scopeFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
                await next.Invoke();
            });

            app.UseSwagger(c => c.SerializeAsV2 = true);
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmployeePortalAPI v1");
            });

            app.UseRouting();

            //app.UseCors("AllowSpecificOrigin");
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

#if !DEBUG
            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "PortalApp";
                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
                }
            });
#endif

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            Task.Run(async () =>
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                    try
                    {
                        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                        var userManager = services.GetRequiredService<UserManager<Employee>>();
                        await EmployeePortalSeeder.SeedRolesAsync(roleManager);
                        await EmployeePortalSeeder.SeedDatabaseAsync(userManager, roleManager);

                        var publicHolidayService = new PublicHolidayService(
                            services.GetRequiredService<IMapper>(),
                            services.GetRequiredService<IPublicHolidayRepository>()
                        );

                        var vacationService = new VacationService(
                            services.GetRequiredService<IVacationRepository>(),
                            services.GetRequiredService<IEmployeeRepository>(),
                            services.GetRequiredService<IAbsenceRepository>(),
                            services.GetRequiredService<IPublicHolidayRepository>(),
                            services.GetRequiredService<IMapper>(),
                            services.GetRequiredService<EmployeePortalContext>(),
                            services.GetRequiredService<IVacationStatusEmailService>()
                        );


                        var employeeService = new EmployeeService(
                            services.GetRequiredService<IMapper>(),
                            services.GetRequiredService<IEmployeeRepository>(),
                            services.GetRequiredService<IVacationRepository>(),
                            services.GetRequiredService<IPasswordHasher<Employee>>()
                        );

                        var absenceService = new AbsenceService(
                            services.GetRequiredService<IAbsenceRepository>(),
                            services.GetRequiredService<IMapper>(),
                            services.GetRequiredService<EmployeePortalContext>()
                        );


                        await EmployeePortalSeeder.SeedPublicHolidays(publicHolidayService);
                        await EmployeePortalSeeder.SeedCompanyVacation(vacationService);
                        await EmployeePortalSeeder.SeedDatabaseAsync(userManager, roleManager);
                        await EmployeePortalSeeder.SeedRolesAsync(roleManager);



                    }
                    catch (Exception ex)
                    {
                        var logger = loggerFactory.CreateLogger<Program>();
                        logger.LogError(ex, "An error occurred while seeding the database.");
                    }
                }
            }).Wait();
        }
    }
}
