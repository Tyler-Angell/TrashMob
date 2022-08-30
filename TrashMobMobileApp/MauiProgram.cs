namespace TrashMobMobileApp
{
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.Configuration;
    using Polly;
    using Polly.Extensions.Http;
    using System.Globalization;
    using System.Net.Http;
    using System.Reflection;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Features.LogOn;
    using TrashMobMobileApp.Models;

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            string strAppConfigStreamName = string.Empty;
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            strAppConfigStreamName = "TrashMobMobileApp.appSettings.Development.json";
#else
            strAppConfigStreamName = "TrashMobMobileApp.appSettings.json";
#endif

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MauiProgram)).Assembly;
            var stream = assembly.GetManifestResourceStream(strAppConfigStreamName);
            builder.Configuration.AddJsonStream(stream);

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            // Add Services
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddSingleton<IContactRequestManager, ContactRequestManager>();
            builder.Services.AddSingleton<IContactRequestRestService, ContactRequestRestService>();
            builder.Services.AddSingleton<IDataStore<Item>, MockDataStore>();
            builder.Services.AddSingleton<IEventAttendeeRestService, EventAttendeeRestService>();
            builder.Services.AddSingleton<IEventSummaryRestService, EventSummaryRestService>();
            builder.Services.AddSingleton<IEventTypeRestService, EventTypeRestService>();
            builder.Services.AddSingleton<IMapRestService, MapRestService>();
            builder.Services.AddSingleton<IMobEventManager, MobEventManager>();
            builder.Services.AddSingleton<IMobEventRestService, MobEventRestService>();
            builder.Services.AddSingleton<IUserManager, UserManager>();
            builder.Services.AddSingleton<IUserRestService, UserRestService>();

            string assemblyName = "TrashMobMobileApp";
            string trashMobApiAddress = builder.Configuration["ApiBaseUrl"];
            B2CConstants b2CConstants = builder.Configuration.GetSection("B2CConstants").Get<B2CConstants>();
            builder.Services.AddSingleton(b2CConstants);

            builder.Services.AddScoped<BaseAddressAuthorizationMessageHandler>();
            builder.Services.AddHttpClient($"{assemblyName}.ServerAPI", client =>
                    client.BaseAddress = new Uri(trashMobApiAddress))
                          .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()
                          .SetHandlerLifetime(TimeSpan.FromMinutes(5));  //Set lifetime to five minutes
                                                                         //                          .AddPolicyHandler(GetRetryPolicy());

            builder.Services.AddHttpClient($"{assemblyName}.ServerAPI.Anonymous", client =>
                  client.BaseAddress = new Uri(trashMobApiAddress))
                  .SetHandlerLifetime(TimeSpan.FromMinutes(5));  //Set lifetime to five minutes
                  // .AddPolicyHandler(GetRetryPolicy());

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{assemblyName}.ServerAPI"));

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{assemblyName}.ServerAPI.Anonymous"));

            builder.Services.AddLogging();
            builder.Services.AddScoped<IErrorBoundaryLogger, CustomBoundaryLogger>();

            return builder.Build();
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
        }
    }

    public class CustomBoundaryLogger : IErrorBoundaryLogger
    {
        public ValueTask LogErrorAsync(Exception exception)
        {
            return ValueTask.CompletedTask;
        }
    }

    public class BaseAddressAuthorizationMessageHandler : DelegatingHandler
    {
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddAuthToken(request);
            return base.Send(request, cancellationToken);
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddAuthToken(request);
            return await base.SendAsync(request, cancellationToken);
        }

        private void AddAuthToken(HttpRequestMessage request)
        {
            request.Headers.Authorization = new System.Net.Http.Headers
                .AuthenticationHeaderValue("bearer", UserState.UserContext.AccessToken);
        }
    }
}