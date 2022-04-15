using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Enigma.String.Tests
{
    public class DependencyInjectionTests
    {
        private readonly IServiceCollection _services;

        public DependencyInjectionTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddEnigmaEncryption_WhenCalled_WithValidParams_ShouldAddedToServiceCollection()
        {
            _services.AddEnigmaEncryption(options =>
            {
                options.SymmetricKey = "2a66161b-e213-4146-85de-826b39194d2c";
            });
            Assert.Equal(1, _services.Count);
        }

        [Fact]
        public void AddEnigmaEncryption_WhenCalled_WithNullOrEmptySymmetricKey_ShouldThrowArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _services.AddEnigmaEncryption(options =>
            {
                options.SymmetricKey = string.Empty;
            }));
            Assert.Equal("you have to provide a symmetric key (Parameter 'SymmetricKey')", exception.Message);
        }
    }
}
