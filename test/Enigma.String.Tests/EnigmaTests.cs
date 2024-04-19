using System;
using Xunit;

namespace Enigma.String.Tests
{
    public class EnigmaTests
    {
        public EnigmaTests()
        {
            var options = new EnigmaOptions("b14ca5898a4e4133bbce2ea2315a1916");
            EnigmaExtension.SetOptions(options);
        }

        [Theory]
        [InlineData("Hello World")]
        public void Encrypt_WhenCall_ShouldReturnAEncryptedValue(string value)
        {
            var encryptedValue = value.Encrypt();
            var isBase64 = encryptedValue.IsBase64String();

            Assert.True(isBase64);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Encrypt_WhenCallWithNullOrEmptyValue_ShouldThrowArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(value.Encrypt);
        }

        [Fact]
        public void Encrypt_WhenCalled_WithNullOrEmptySymmetricKey_ShouldThrowArgumentNullException()
        {
            var value = "this-is-my-value";
            EnigmaExtension.SetOptions(new EnigmaOptions(string.Empty));
            Assert.Throws<ArgumentNullException>(value.Encrypt);
        }

        [Theory]
        [InlineData("Bypc0t1hr4vVC0W3KNIyhA==")]
        public void Decrypt_WhenCalled_ShouldReturnNormalStringValue(string value)
        {
            var decryptValue = value.Decrypt();
            var isBase64 = decryptValue.IsBase64String();

            Assert.Equal("Hello World", decryptValue);
            Assert.False(isBase64);
        }

        [Theory]
        [InlineData("this is not base 64 string")]
        public void Decrypt_WhenCalledWithNoneBase64String_ShouldThrowArgumentException(string value)
        {
            var isBase64 = value.IsBase64String();
            Assert.False(isBase64);
            Assert.Throws<ArgumentException>(value.Decrypt);
        }

        [Fact]
        public void Decrypt_WhenCalledWithNullOrEmptyString_ShouldThrowArgumentException()
        {
            var value = "";
            var exception = Assert.Throws<ArgumentException>(value.Decrypt);
            Assert.Equal($"the value '{value}' should not be null or empty", exception.Message);
        }

        [Fact]
        public void Decrypt_WhenCalledWithNullOrEmptySymmetricKey_ShouldThrowArgumentException()
        {
            EnigmaExtension.SetOptions(new EnigmaOptions(""));
            var value = "Bypc0t1hr4vVC0W3KNIyhA==";
            var exception = Assert.Throws<ArgumentNullException>(value.Decrypt);
            Assert.Equal("Value cannot be null. (Parameter 'SymmetricKey')", exception.Message);
        }
    }
}