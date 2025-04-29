using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace SecurityAndCompliance.Tests
{
    public class SecurityAndComplianceTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public SecurityAndComplianceTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        [Fact]
        public async Task Test_TLS_Configuration()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("TLS 1.3", response.Version.ToString());
        }

        [Fact]
        public void Test_Encryption_Configuration()
        {
            // Arrange
            var encryptionKey = _configuration["Encryption:Key"];

            // Act & Assert
            using (var aes = Aes.Create())
            {
                Assert.Equal(256, aes.KeySize);
                Assert.Equal(CipherMode.CBC, aes.Mode);
                Assert.Equal(PaddingMode.PKCS7, aes.Padding);
            }
        }

        [Fact]
        public void Test_Password_Hashing()
        {
            // Arrange
            var password = "TestPassword123!";
            var salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(salt);

            // Act
            var hashedPassword = HashPassword(password, salt);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.Equal(64, hashedPassword.Length); // SHA-256 produces 64-character hex string
        }

        [Fact]
        public async Task Test_Authentication_Headers()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/secure");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.True(response.Headers.Contains("X-Content-Type-Options"));
            Assert.True(response.Headers.Contains("X-Frame-Options"));
            Assert.True(response.Headers.Contains("X-XSS-Protection"));
            Assert.True(response.Headers.Contains("Strict-Transport-Security"));
        }

        [Fact]
        public void Test_Compliance_Configuration()
        {
            // Arrange
            var complianceSettings = _configuration.GetSection("Compliance");

            // Assert
            Assert.True(bool.Parse(complianceSettings["EnableAuditLogging"]));
            Assert.True(bool.Parse(complianceSettings["EnableDataEncryption"]));
            Assert.True(bool.Parse(complianceSettings["EnableAccessControl"]));
            Assert.Equal("90", complianceSettings["AuditLogRetentionDays"]);
        }

        [Fact]
        public void Test_GDPR_Compliance()
        {
            // Arrange
            var gdprSettings = _configuration.GetSection("GDPR");

            // Assert
            Assert.True(bool.Parse(gdprSettings["EnableDataMinimization"]));
            Assert.True(bool.Parse(gdprSettings["EnableRightToErasure"]));
            Assert.True(bool.Parse(gdprSettings["EnableDataPortability"]));
            Assert.Equal("30", gdprSettings["DataRetentionDays"]);
        }

        [Fact]
        public void Test_PCI_DSS_Compliance()
        {
            // Arrange
            var pciSettings = _configuration.GetSection("PCI_DSS");

            // Assert
            Assert.True(bool.Parse(pciSettings["EnableTokenization"]));
            Assert.True(bool.Parse(pciSettings["EnableNetworkSegmentation"]));
            Assert.True(bool.Parse(pciSettings["EnableVulnerabilityScanning"]));
            Assert.Equal("AES-256", pciSettings["EncryptionAlgorithm"]);
        }

        [Fact]
        public void Test_NIST_Compliance()
        {
            // Arrange
            var nistSettings = _configuration.GetSection("NIST");

            // Assert
            Assert.True(bool.Parse(nistSettings["EnableMultiFactorAuth"]));
            Assert.True(bool.Parse(nistSettings["EnableSessionTimeout"]));
            Assert.True(bool.Parse(nistSettings["EnableAccessControl"]));
            Assert.Equal("15", nistSettings["SessionTimeoutMinutes"]);
        }

        private string HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var saltedPassword = new byte[passwordBytes.Length + salt.Length];
                
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                var hash = sha256.ComputeHash(saltedPassword);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
} 