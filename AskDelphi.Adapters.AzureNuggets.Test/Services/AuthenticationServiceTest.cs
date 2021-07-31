using AskDelphi.Adapter.AzureNuggets;
using AskDelphi.Adapter.AzureNuggets.DTO;
using AskDelphi.Adapter.AzureNuggets.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskDelphi.Adapters.AzureNuggets.Services.Test
{
    // $ figlet refresh | sed 's/^/\/\/      /'
    [TestClass]
    public class AuthenticationServiceTest
    {
        //       _             _
        //      | | ___   __ _(_)_ __
        //      | |/ _ \ / _` | | '_ \
        //      | | (_) | (_| | | | | |
        //      |_|\___/ \__, |_|_| |_|
        //               |___/

        [TestMethod]
        public async Task WHEN_LoginWithoutAuthHeader_THEN_LoginFails()
        {
            // Setup
            var loggerMoq = new Mock<ILogger<AuthenticationService>>();
            var configurationMoq = new Mock<IConfiguration>();
            var tokenServiceMoq = new Mock<ITokenService>();
            var authenticationService = new AuthenticationService(loggerMoq.Object, configurationMoq.Object, tokenServiceMoq.Object);
            var operationContext = new OperationContextFactory().CreateBackgroundOperationContext();
            var response = new AuthLoginSuccessResponse(operationContext);

            // Header without any values
            var header = new KeyValuePair<string, StringValues>("Authorization", new StringValues());
            var result = await authenticationService.Login(operationContext, response, header, null);
            
            // Assert
            result.Should().BeFalse("Expect login to fail when there is no authorization header.");
        }

        [TestMethod]
        public async Task WHEN_LoginWithTooManyAuthHeaders_THEN_LoginFails()
        {
            // Setup
            var loggerMoq = new Mock<ILogger<AuthenticationService>>();
            var configurationMoq = new Mock<IConfiguration>();
            var tokenServiceMoq = new Mock<ITokenService>();
            var authenticationService = new AuthenticationService(loggerMoq.Object, configurationMoq.Object, tokenServiceMoq.Object);
            var operationContext = new OperationContextFactory().CreateBackgroundOperationContext();
            var response = new AuthLoginSuccessResponse(operationContext);

            // Header with multiple values
            var header = new KeyValuePair<string, StringValues>("Authorization", new StringValues(new string[] { "a", "b" } ));
            var result = await authenticationService.Login(operationContext, response, header, null);

            // Assert
            result.Should().BeFalse("Expect login to fail when there is more then one authorization header.");
        }

        [TestMethod]
        public async Task WHEN_LoginWithEmptyAuthHeader_THEN_LoginFails()
        {
            // Setup
            var loggerMoq = new Mock<ILogger<AuthenticationService>>();
            var configurationMoq = new Mock<IConfiguration>();
            var tokenServiceMoq = new Mock<ITokenService>();
            var authenticationService = new AuthenticationService(loggerMoq.Object, configurationMoq.Object, tokenServiceMoq.Object); var operationContext = new OperationContextFactory().CreateBackgroundOperationContext();
            var response = new AuthLoginSuccessResponse(operationContext);

            // Header with empty value
            var header = new KeyValuePair<string, StringValues>("Authorization", new StringValues(""));
            var result = await authenticationService.Login(operationContext, response, header, null);

            // Assert
            result.Should().BeFalse("Expect login to fail when there is an empty authorization header.");
        }

        [TestMethod]
        public async Task WHEN_LoginWithAuthHeaderWithoutPurpose_THEN_LoginFails()
        {
            // Setup
            var configurationMoq = new Mock<IConfiguration>();
            var loggerMoq = new Mock<ILogger<AuthenticationService>>();
            var tokenServiceMoq = new Mock<ITokenService>();
            var authenticationService = new AuthenticationService(loggerMoq.Object, configurationMoq.Object, tokenServiceMoq.Object);
            var operationContext = new OperationContextFactory().CreateBackgroundOperationContext();
            var response = new AuthLoginSuccessResponse(operationContext);

            // Header with value but without a purpose indication
            var header = new KeyValuePair<string, StringValues>("Authorization", new StringValues("abc123"));
            var result = await authenticationService.Login(operationContext, response, header, null);

            // Assert
            result.Should().BeFalse("Expect login to fail when there is an authorization header without a purpose.");
        }

        [TestMethod]
        public async Task WHEN_LoginWithAuthHeaderWithUnsupportedPurpose_THEN_LoginFails()
        {
            // Setup
            Mock<IConfiguration> configurationMoq = CreateMockConfiguration(string.Empty);
            var loggerMoq = new Mock<ILogger<AuthenticationService>>();
            var tokenServiceMoq = new Mock<ITokenService>();
            var authenticationService = new AuthenticationService(loggerMoq.Object, configurationMoq.Object, tokenServiceMoq.Object);
            var operationContext = new OperationContextFactory().CreateBackgroundOperationContext();
            var response = new AuthLoginSuccessResponse(operationContext);

            // Header with purpose and value, but not a supported purpose
            var header = new KeyValuePair<string, StringValues>("Authorization", new StringValues("XXX:abc123"));
            var result = await authenticationService.Login(operationContext, response, header, null);

            // Assert
            result.Should().BeFalse("Expect login to fail when there is an authorization header with an unsupported a purpose.");
        }

        [TestMethod]
        public async Task WHEN_LoginWithAuthHeaderWithSupportedPurposeButBadKey_THEN_LoginFails()
        {
            // Setup
            Mock<IConfiguration> configurationMoq = CreateMockConfiguration(string.Empty);
            var loggerMoq = new Mock<ILogger<AuthenticationService>>();
            var tokenServiceMoq = new Mock<ITokenService>();
            var authenticationService = new AuthenticationService(loggerMoq.Object, configurationMoq.Object, tokenServiceMoq.Object);
            var operationContext = new OperationContextFactory().CreateBackgroundOperationContext();
            var response = new AuthLoginSuccessResponse(operationContext);

            // Header with purpose and invalid key
            var header = new KeyValuePair<string, StringValues>("Authorization", new StringValues("Purpose:abc1234"));
            var result = await authenticationService.Login(operationContext, response, header, null);

            // Assert
            result.Should().BeFalse("Expect login to fail when there is an authorization header with the wrong key for the purpose.");
        }

        [TestMethod]
        public async Task WHEN_LoginWithAuthHeaderWithSupportedPurposeAndCorrectKey_THEN_LoginSucceeds()
        {
            // Setup
            Mock<IConfiguration> configurationMoq = CreateMockConfiguration("abc123");
            var tokenServiceMoq = new Mock<ITokenService>();
            var loggerMoq = new Mock<ILogger<AuthenticationService>>();
            tokenServiceMoq.Setup(x => x.GenerateToken(It.IsAny<IOperationContext>(), It.IsAny<string>(), It.IsAny<ClaimTuple[]>())).Returns(async () => await Task.FromResult<string>("token"));
            var authenticationService = new AuthenticationService(loggerMoq.Object, configurationMoq.Object, tokenServiceMoq.Object);
            var operationContext = new OperationContextFactory().CreateBackgroundOperationContext();
            var response = new AuthLoginSuccessResponse(operationContext);

            // Header with purpose and valid key
            var header = new KeyValuePair<string, StringValues>("Authorization", new StringValues("Purpose:abc123"));
            var result = await authenticationService.Login(operationContext, response, header, null);

            // Assert
            result.Should().BeTrue("Expect login to succeed.");
            response.Token.Should().Be("token", "Expect token to be set");
        }

        //       _                         _
        //      | | ___   __ _  ___  _   _| |_
        //      | |/ _ \ / _` |/ _ \| | | | __|
        //      | | (_) | (_| | (_) | |_| | |_
        //      |_|\___/ \__, |\___/ \__,_|\__|
        //               |___/

        [TestMethod]
        public async Task WHEN_LogoutInvoked_THEN_TokenServiceLogoutIsInvoked()
        {
            // Setup
            Mock<IConfiguration> configurationMoq = CreateMockConfiguration("abc123");
            var loggerMoq = new Mock<ILogger<AuthenticationService>>();
            var tokenServiceMoq = new Mock<ITokenService>();
            var authenticationService = new AuthenticationService(loggerMoq.Object, configurationMoq.Object, tokenServiceMoq.Object);
            var operationContext = new OperationContextFactory().CreateBackgroundOperationContext();
            var response = new AuthLogoutSuccessResponse(operationContext);

            // Act
            var result = await authenticationService.Logout(operationContext, response);

            // Assert
            tokenServiceMoq.Verify((x) => x.InvalidateToken(It.IsAny<IOperationContext>()), Times.Once, "Expecting InvalidateToken to be called for this operation context exactly once upon logout.");
        }

        //                 __               _
        //       _ __ ___ / _|_ __ ___  ___| |__
        //      | '__/ _ \ |_| '__/ _ \/ __| '_ \
        //      | | |  __/  _| | |  __/\__ \ | | |
        //      |_|  \___|_| |_|  \___||___/_| |_|
        //

        [TestMethod]
        public async Task WHEN_RefreshInvoked_THEN_TokenServiceRefreshIsInvoked()
        {
            // Setup
            Mock<IConfiguration> configurationMoq = CreateMockConfiguration("abc123");
            var tokenServiceMoq = new Mock<ITokenService>();
            var loggerMoq = new Mock<ILogger<AuthenticationService>>();
            tokenServiceMoq.Setup(x => x.Refresh(It.IsAny<IOperationContext>(), It.IsAny<string>())).Returns(async () => await Task.FromResult("valid token"));
            var authenticationService = new AuthenticationService(loggerMoq.Object, configurationMoq.Object, tokenServiceMoq.Object);
            var operationContext = new OperationContextFactory().CreateBackgroundOperationContext();
            var response = new AuthRefreshSuccessResponse(operationContext);

            // Act
            var result = await authenticationService.Refresh(operationContext, response, "any refreshtoken");

            // Assert
            tokenServiceMoq.Verify((x) => x.Refresh(It.IsAny<IOperationContext>(), It.Is<string>(s => s == "any refreshtoken")), Times.Once, "Expecting RefreshToken() to be invoked exactly once.");
            result.Should().BeTrue("Expect authentication service refresh to return a true value when a valid refresh token is returned");
            response.Token.Should().Be("valid token", "Expect operation response to be updated for the valid token that was returned");
        }

        [TestMethod]
        public async Task WHEN_RefreshInvoked_AND_TokenServiceRefreshReturnsNull_THEN_TokenServiceFails()
        {
            // Setup
            Mock<IConfiguration> configurationMoq = CreateMockConfiguration("abc123");
            var tokenServiceMoq = new Mock<ITokenService>();
            var loggerMoq = new Mock<ILogger<AuthenticationService>>();
            tokenServiceMoq.Setup(x => x.Refresh(It.IsAny<IOperationContext>(), It.IsAny<string>())).Returns(async () => await Task.FromResult<string>(null));
            var authenticationService = new AuthenticationService(loggerMoq.Object, configurationMoq.Object, tokenServiceMoq.Object);
            var operationContext = new OperationContextFactory().CreateBackgroundOperationContext();
            var response = new AuthRefreshSuccessResponse(operationContext);

            // Act
            var result = await authenticationService.Refresh(operationContext, response, "any refreshtoken");

            // Assert
            tokenServiceMoq.Verify((x) => x.Refresh(It.IsAny<IOperationContext>(), It.Is<string>(s => s == "any refreshtoken")), Times.Once, "Expecting RefreshToken() to be invoked exactly once.");
            result.Should().BeFalse("Expect authentication service refresh to return a false value when no valid refresh token is returned");
            response.Token.Should().BeNull("Expect operation response to include null value for the token when refresh fails");
        }


        //                                        _                   _
        //       ___ _   _ _ __  _ __   ___  _ __| |_    ___ ___   __| | ___
        //      / __| | | | '_ \| '_ \ / _ \| '__| __|  / __/ _ \ / _` |/ _ \
        //      \__ \ |_| | |_) | |_) | (_) | |  | |_  | (_| (_) | (_| |  __/
        //      |___/\__,_| .__/| .__/ \___/|_|   \__|  \___\___/ \__,_|\___|
        //                |_|   |_|

        private static Mock<IConfiguration> CreateMockConfiguration(string result)
        {
            var configurationSectionMoq = new Mock<IConfigurationSection>();
            configurationSectionMoq.Setup(x => x.Value).Returns(() => result);

            var configurationMoq = new Mock<IConfiguration>();
            configurationMoq.Setup(c => c.GetSection(It.IsAny<string>())).Returns(configurationSectionMoq.Object);
            return configurationMoq;
        }
    }
}
