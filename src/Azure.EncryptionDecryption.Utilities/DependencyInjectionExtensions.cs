using Azure.EncryptionDecryption.Utilities.Contracts;
using Azure.EncryptionDecryption.Utilities.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Azure.EncryptionDecryption.Utilities
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddUtilities(this IServiceCollection services)
        {
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IDecryptionService, DecryptionService>();

            return services;
        }
    }
}