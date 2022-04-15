using Microsoft.Extensions.DependencyInjection;

namespace Enigma.String
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEnigmaEncryption(this IServiceCollection services,
            Action<EnigmaOptions> configure)
        {
            var options = new EnigmaOptions();
            configure.Invoke(options);
            if (string.IsNullOrEmpty(options.SymmetricKey))
                throw new ArgumentNullException(nameof(options.SymmetricKey), "you have to provide a symmetric key");
            services.AddSingleton(options);
            EnigmaExtension.SetOptions(options);
            return services;
        }
    }
}
