using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Aaina.Service;

using Aaina.Web.Code;
using Aaina.Web.Code.LIBS;
using Aaina.Web.Code.Validation;
using Aaina.Web.Code.Validation.Account;
using Aaina.Web.Models;
using Aaina.Web.Models.Hubs;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Wkhtmltopdf.NetCore;
using ApplicationDbContext = Aaina.Data.Models.ApplicationDbContext;

namespace Aaina.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(this.Configuration.GetConnectionString("AainaDb")));
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.Configure<FormOptions>(x => x.ValueCountLimit = 13313);
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);//You can set Time   
            });
            services.AddMvc().AddFluentValidation().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSession();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.Configure<CookieAuthenticationOptions>(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
                //options.CookieHttpOnly = true;
                options.Cookie.HttpOnly = true;
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.SlidingExpiration = true;
            });

            services.Configure<RequestLocalizationOptions>(
                            options =>
                            {
                                var supportedCultures = new List<CultureInfo>
                                    {
                            new CultureInfo("en-GB")
                                    };

                                options.DefaultRequestCulture = new RequestCulture(culture: "en-GB", uiCulture: "en-GB");
                                options.SupportedCultures = supportedCultures;
                                options.SupportedUICultures = supportedCultures;
                            });
            services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });


            //services.AddSingleton<IJobFactory, CustomQuartzJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            //services.AddSingleton<NotificationJob>();
            //services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(NotificationJob), "Notification Job", "0/59 * * * * ?"));
            //services.AddHostedService<CustomQuartzHostedService>();


            InitServices(services);

            InitValidation(services);
            services.AddWkhtmltopdf();
            services.AddSignalR();
            SiteKeys.Configure(Configuration.GetSection("AppSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // For getting IP address
            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto });
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
                app.UseExceptionHandler(a => a.Run(async context =>
                {
                    if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature.Error;
                        var result = JsonConvert.SerializeObject(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(result);
                    }
                    else
                    {
                        //app.UseDeveloperExceptionPage();
                        app.UseExceptionHandler("/Home/Error");
                    }
                }));
            }
            else
            {

                //app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };
            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/ChatHub");
                endpoints.MapHub<NotificationUserHub>("/NotificationUserHub");
                endpoints.MapAreaControllerRoute(
                 name: "SuperAdmin",
                 areaName: "SuperAdmin",
                 pattern: "SuperAdmin/{controller}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                name: "Admin",
                areaName: "Admin",
                pattern: "Admin/{controller}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "gameRoute",
                    pattern: "{tenant}/{controller=project}/{action=gamefeebback}/{id?}");

                endpoints.MapRazorPages();
            });
            ContextProvider.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        }


        private void InitServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IUserLoginService, UserLoginService>();
            services.AddTransient<IWeightageService, WeightageService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<IAttributeService, AttributeService>();
            services.AddTransient<ILookService, LookService>();
            services.AddTransient<IGameFeedbackService, GameFeedbackService>();
            services.AddTransient<IPushNotificationService, PushNotificationService>();
            services.AddTransient<IWeightagePresetService, WeightagePresetService>();
            services.AddTransient<IFilterService, FilterService>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IPlayService, PlayService>();
            services.AddTransient<IStatusService, StatusService>();
            services.AddTransient<IFormBuilderService, FormBuilderService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IPreSessionService, PreSessionService>();
            services.AddTransient<IPollService, PollService>();
            services.AddTransient<IPostSessionService, PostSessionService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IRoleMenuPermissionService, RoleMenuPermissionService>();
            services.AddTransient<IPreSessionGroupService, PreSessionGroupService>();
            services.AddTransient<IGoogleDriveService, GoogleDriveService>();
            services.AddTransient<IDropBoxService, DropBoxService>();
            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<IViewRender, ViewRender>();
            services.AddTransient<IUserMenuPermissionService, UserMenuPermissionService>();

            //services.AddDbContext<DbContext>(ServiceLifetime.Transient);

        }

        public void InitValidation(IServiceCollection services)
        {
            services.AddTransient<IValidator<LoginDto>, LoginDtoValidator>();
            services.AddTransient<IValidator<RegisterDto>, RegisterDtoValidator>();
            services.AddTransient<IValidator<PasswordResetDto>, PasswordResetDtoValidator>();
            services.AddTransient<IValidator<ChangePasswordDto>, ChangePasswordDtoValidator>();
            services.AddTransient<IValidator<GameDto>, GameDtoValidator>();
            services.AddTransient<IValidator<TeamDto>, TeamDtoValidator>();
            services.AddTransient<IValidator<UserProfileDto>, UserProfileDtoValidator>();
            services.AddTransient<IValidator<LookDto>, LookDtoValidator>();
            services.AddTransient<IValidator<NotificationModel>, NotificationDtoValidator>();
            services.AddTransient<IValidator<PlayDto>, PlayDtoValidator>();
            services.AddTransient<IValidator<StatusDto>, StatusDtoValidator>();
        }

    }
}
