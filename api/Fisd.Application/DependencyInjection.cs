using Fisd.Application.Email;
using Fisd.Application.Security;
using Fisd.Application.Services;
using Fisd.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Fisd.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFisdApplication(this IServiceCollection services)
        {
            services.AddScoped<IArticlesService, ArticlesService>();
            services.AddScoped<IContactsService, ContactsService>();
            services.AddScoped<ISouvenirsService, SouvenirsService>();
            services.AddScoped<ITestimonialsService, TestimonialsService>();
            services.AddScoped<IEditionsService, EditionsService>();
            services.AddScoped<ISlidesService, SlidesService>();
            services.AddScoped<IPillarsService, PillarsService>();
            services.AddScoped<IParticipantsService, ParticipantsService>();
            services.AddScoped<ISpeakersService, SpeakersService>();
            services.AddScoped<IPartnersService, PartnersService>();
            services.AddScoped<IEngagementActionsService, EngagementActionsService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminUsersService, AdminUsersService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<ISiteSettingsService, SiteSettingsService>();
            services.AddScoped<IGlobalEmailService, GlobalEmailService>();
            services.AddScoped<IErrorLogService, ErrorLogService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<ISystemHealthService, SystemHealthService>();
            services.AddScoped<IHelpArticlesService, HelpArticlesService>();
            services.AddScoped<ITicketsService, TicketsService>();
            services.AddScoped<IEmailHistoryService, EmailHistoryService>();
            services.AddSingleton<JwtTokenGenerator>();
            return services;
        }
    }
}
