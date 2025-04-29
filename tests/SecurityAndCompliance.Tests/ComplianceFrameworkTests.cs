using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace SecurityAndCompliance.Tests
{
    public class ComplianceFrameworkTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public ComplianceFrameworkTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        [Fact]
        public async Task Test_NIST_800_53_Compliance()
        {
            // Access Control (AC)
            var acSettings = _configuration.GetSection("NIST:AccessControl");
            Assert.True(bool.Parse(acSettings["EnableRBAC"]));
            Assert.True(bool.Parse(acSettings["EnableLeastPrivilege"]));
            Assert.True(bool.Parse(acSettings["EnableSessionLock"]));

            // Audit and Accountability (AU)
            var auSettings = _configuration.GetSection("NIST:Audit");
            Assert.True(bool.Parse(auSettings["EnableAuditLogging"]));
            Assert.Equal("90", auSettings["LogRetentionDays"]);
            Assert.True(bool.Parse(auSettings["EnableAuditReview"]));

            // System and Communications Protection (SC)
            var scSettings = _configuration.GetSection("NIST:Communications");
            Assert.True(bool.Parse(scSettings["EnableTLS"]));
            Assert.Equal("1.3", scSettings["TLSVersion"]);
            Assert.True(bool.Parse(scSettings["EnableNetworkSegmentation"]));
        }

        [Fact]
        public async Task Test_HIPAA_Compliance()
        {
            // Technical Safeguards
            var techSettings = _configuration.GetSection("HIPAA:Technical");
            Assert.True(bool.Parse(techSettings["EnableEncryption"]));
            Assert.True(bool.Parse(techSettings["EnableAccessControl"]));
            Assert.True(bool.Parse(techSettings["EnableAuditControls"]));

            // Physical Safeguards
            var physicalSettings = _configuration.GetSection("HIPAA:Physical");
            Assert.True(bool.Parse(physicalSettings["EnableWorkstationSecurity"]));
            Assert.True(bool.Parse(physicalSettings["EnableDeviceMediaControls"]));

            // Administrative Safeguards
            var adminSettings = _configuration.GetSection("HIPAA:Administrative");
            Assert.True(bool.Parse(adminSettings["EnableSecurityManagement"]));
            Assert.True(bool.Parse(adminSettings["EnableSecurityIncidentProcedures"]));
            Assert.True(bool.Parse(adminSettings["EnableContingencyPlan"]));
        }

        [Fact]
        public async Task Test_PCI_DSS_Compliance()
        {
            // Build and Maintain a Secure Network
            var networkSettings = _configuration.GetSection("PCI:Network");
            Assert.True(bool.Parse(networkSettings["EnableFirewall"]));
            Assert.True(bool.Parse(networkSettings["EnableVendorDefaults"]));

            // Protect Cardholder Data
            var dataSettings = _configuration.GetSection("PCI:DataProtection");
            Assert.True(bool.Parse(dataSettings["EnableEncryption"]));
            Assert.True(bool.Parse(dataSettings["EnableTokenization"]));
            Assert.True(bool.Parse(dataSettings["EnableMasking"]));

            // Maintain a Vulnerability Management Program
            var vulnSettings = _configuration.GetSection("PCI:Vulnerability");
            Assert.True(bool.Parse(vulnSettings["EnableAntiVirus"]));
            Assert.True(bool.Parse(vulnSettings["EnableSecureSystems"]));
            Assert.True(bool.Parse(vulnSettings["EnableVulnerabilityScanning"]));
        }

        [Fact]
        public async Task Test_GDPR_Compliance()
        {
            // Data Protection Principles
            var principlesSettings = _configuration.GetSection("GDPR:Principles");
            Assert.True(bool.Parse(principlesSettings["EnableLawfulness"]));
            Assert.True(bool.Parse(principlesSettings["EnableFairness"]));
            Assert.True(bool.Parse(principlesSettings["EnableTransparency"]));
            Assert.True(bool.Parse(principlesSettings["EnablePurposeLimitation"]));
            Assert.True(bool.Parse(principlesSettings["EnableDataMinimization"]));
            Assert.True(bool.Parse(principlesSettings["EnableAccuracy"]));
            Assert.True(bool.Parse(principlesSettings["EnableStorageLimitation"]));
            Assert.True(bool.Parse(principlesSettings["EnableIntegrity"]));
            Assert.True(bool.Parse(principlesSettings["EnableConfidentiality"]));

            // Data Subject Rights
            var rightsSettings = _configuration.GetSection("GDPR:Rights");
            Assert.True(bool.Parse(rightsSettings["EnableRightToAccess"]));
            Assert.True(bool.Parse(rightsSettings["EnableRightToRectification"]));
            Assert.True(bool.Parse(rightsSettings["EnableRightToErasure"]));
            Assert.True(bool.Parse(rightsSettings["EnableRightToRestriction"]));
            Assert.True(bool.Parse(rightsSettings["EnableRightToDataPortability"]));
            Assert.True(bool.Parse(rightsSettings["EnableRightToObject"]));
            Assert.True(bool.Parse(rightsSettings["EnableRightToAutomatedDecision"]));
        }

        [Fact]
        public async Task Test_SOC_2_Compliance()
        {
            // Security
            var securitySettings = _configuration.GetSection("SOC2:Security");
            Assert.True(bool.Parse(securitySettings["EnableAccessControl"]));
            Assert.True(bool.Parse(securitySettings["EnableChangeManagement"]));
            Assert.True(bool.Parse(securitySettings["EnableLogicalAccess"]));

            // Availability
            var availabilitySettings = _configuration.GetSection("SOC2:Availability");
            Assert.True(bool.Parse(availabilitySettings["EnableBackup"]));
            Assert.True(bool.Parse(availabilitySettings["EnableDisasterRecovery"]));
            Assert.True(bool.Parse(availabilitySettings["EnableBusinessContinuity"]));

            // Processing Integrity
            var integritySettings = _configuration.GetSection("SOC2:Integrity");
            Assert.True(bool.Parse(integritySettings["EnableQualityAssurance"]));
            Assert.True(bool.Parse(integritySettings["EnableProcessMonitoring"]));
            Assert.True(bool.Parse(integritySettings["EnableErrorHandling"]));

            // Confidentiality
            var confidentialitySettings = _configuration.GetSection("SOC2:Confidentiality");
            Assert.True(bool.Parse(confidentialitySettings["EnableDataClassification"]));
            Assert.True(bool.Parse(confidentialitySettings["EnableEncryption"]));
            Assert.True(bool.Parse(confidentialitySettings["EnableAccessRestrictions"]));

            // Privacy
            var privacySettings = _configuration.GetSection("SOC2:Privacy");
            Assert.True(bool.Parse(privacySettings["EnableNotice"]));
            Assert.True(bool.Parse(privacySettings["EnableChoice"]));
            Assert.True(bool.Parse(privacySettings["EnableCollection"]));
            Assert.True(bool.Parse(privacySettings["EnableUse"]));
            Assert.True(bool.Parse(privacySettings["EnableDisclosure"]));
            Assert.True(bool.Parse(privacySettings["EnableSecurity"]));
            Assert.True(bool.Parse(privacySettings["EnableQuality"]));
            Assert.True(bool.Parse(privacySettings["EnableMonitoring"]));
        }
    }
} 