namespace TrashMob
{
    using Azure.Identity;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Identity.Web;
    using Microsoft.OpenApi.Models;
    using System;
    using System.Text.Json.Serialization;
    using TrashMob.Models;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Partners;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Persistence.Events;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Persistence.Partners;

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            CurrentEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(options => {
                Configuration.Bind("AzureAdB2C", options);

                options.TokenValidationParameters.NameClaimType = "name";
            },

            options => { Configuration.Bind("AzureAdB2C", options); });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "client-app/build";
            });

            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddDbContext<MobDbContext>(c => c.UseLazyLoadingProxies());

            // Non-patterned
            services.AddScoped<IDocusignManager, DocusignManager>();
            services.AddScoped<IEmailManager, EmailManager>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IMapRepository, MapRepository>();
            services.AddScoped<ISecretRepository, SecretRepository>();
            services.AddScoped<INotificationManager, NotificationManager>();

            // Migrated Repositories
            services.AddScoped<IKeyedRepository<ContactRequest>, KeyedRepository<ContactRequest>>();
            services.AddScoped<IKeyedRepository<Partner>, KeyedRepository<Partner>>();
            services.AddScoped<IKeyedRepository<PartnerContact>, KeyedRepository<PartnerContact>>();
            services.AddScoped<IKeyedRepository<PartnerDocument>, KeyedRepository<PartnerDocument>>();
            services.AddScoped<IKeyedRepository<PartnerRequest>, KeyedRepository<PartnerRequest>>();
            services.AddScoped<IKeyedRepository<PartnerSocialMediaAccount>, KeyedRepository<PartnerSocialMediaAccount>>();
            services.AddScoped<ILookupRepository<PartnerType>, LookupRepository<PartnerType>>();
            services.AddScoped<IBaseRepository<PartnerUser>, BaseRepository<PartnerUser>>();
            services.AddScoped<ILookupRepository<ServiceType>, LookupRepository<ServiceType>>();
            services.AddScoped<ILookupRepository<SocialMediaAccountType>, LookupRepository<SocialMediaAccountType>>();

            // Migrated Managers
            services.AddScoped<IKeyedManager<Partner>, PartnerManager>();
            services.AddScoped<IKeyedManager<PartnerDocument>, PartnerDocumentManager>();
            services.AddScoped<IKeyedManager<PartnerContact>, PartnerContactManager>();
            services.AddScoped<IKeyedManager<PartnerSocialMediaAccount>, PartnerSocialMediaAccountManager>();
            services.AddScoped<ILookupManager<PartnerType>, PartnerTypeManager>();
            services.AddScoped<IBaseManager<PartnerUser>, PartnerUserManager>();
            services.AddScoped<IKeyedManager<ContactRequest>, ContactRequestManager>();
            services.AddScoped<ILookupManager<ServiceType>, ServiceTypeManager>();
            services.AddScoped<ILookupManager<SocialMediaAccountType>, SocialMediaAccountTypeManager>();

            // Intentional deviation due to special methods
            services.AddScoped<IPartnerRequestManager, PartnerRequestManager>();

            // Not Migrated Repositories and Managers
            services.AddScoped<IEventAttendeeRepository, EventAttendeeRepository>();
            services.AddScoped<IEventPartnerRepository, EventPartnerRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventPartnerStatusRepository, EventPartnerStatusRepository>();
            services.AddScoped<IEventStatusRepository, EventStatusRepository>();
            services.AddScoped<IEventSummaryRepository, EventSummaryRepository>();
            services.AddScoped<IEventTypeRepository, EventTypeRepository>();
            services.AddScoped<IMessageRequestManager, MessageRequestManager>();
            services.AddScoped<IMessageRequestRepository, MessageRequestRepository>();
            services.AddScoped<INonEventUserNotificationRepository, NonEventUserNotificationRepository>();
            services.AddScoped<IPartnerLocationRepository, PartnerLocationRepository>();
            services.AddScoped<IPartnerRepository, PartnerRepository>();
            services.AddScoped<IPartnerRequestRepository, PartnerRequestRepository>();
            services.AddScoped<IPartnerStatusRepository, PartnerStatusRepository>();
            services.AddScoped<IPartnerRequestStatusRepository, PartnerRequestStatusRepository>();
            services.AddScoped<IPartnerUserRepository, PartnerUserRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

            services.AddDatabaseDeveloperPageExceptionFilter();
            
            if (CurrentEnvironment.IsDevelopment())
            {
                services.AddScoped<IKeyVaultManager, LocalKeyVaultManager>();
                services.AddScoped<IDocusignAuthenticator, DocusignStringAuthenticator>();
            }
            else
            {
                services.AddAzureClients(azureClientFactoryBuilder =>
                {
                    azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential());
                    azureClientFactoryBuilder.AddSecretClient(Configuration.GetValue<Uri>("VaultUri"));
                });

                services.AddScoped<IKeyVaultManager, KeyVaultManager>();
                services.AddScoped<IDocusignAuthenticator, DocusignStringAuthenticator>();
            }

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "trashmobapi", Version = "v1" });
            });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "trashmobapi v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client-app";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
