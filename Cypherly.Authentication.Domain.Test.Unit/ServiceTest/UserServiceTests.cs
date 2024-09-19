using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using FluentAssertions;

namespace Cypherly.Authentication.Domain.Test.Unit.ServiceTest
{
    public class UserServiceTests
    {
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userService = new UserService();
        }

        [Fact]
        public void CreateUser_Should_Fail_When_Email_Is_Invalid()
        {
            // Arrange
            string invalidEmail = "invalidemail";
            string password = "Password123!";

            // Act
            var result = _userService.CreateUser(invalidEmail, password);

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Be("Invalid email address."); // Adjust based on actual message in Email.Create
        }

        [Fact]
        public void CreateUser_Should_Fail_When_Password_Is_Invalid()
        {
            // Arrange
            var email = "test@mail.com";
            var invalidPassword = "short"; // Assuming password should meet certain criteria

            // Act
            var result = _userService.CreateUser(email, invalidPassword);

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Contain("Incorrect password:");
        }

        [Fact]
        public void CreateUser_Should_Succeed_When_Valid_Email_And_Password_Are_Provided()
        {
            // Arrange
            var email = "test@mail.com";
            var password = "Password123!";

            // Act
            var result = _userService.CreateUser(email, password);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Email.Address.Should().Be(email);
            result.Value.IsVerified.Should().BeFalse();
            result.Value.VerificationCodes.Should().HaveCount(1);
        }

        [Fact]
        public void CreateUser_Should_Add_UserCreatedEvent()
        {
            // Arrange
            var email = "test@mail.com";
            var password = "Password123!";

            // Act
            var result = _userService.CreateUser(email, password);

            // Assert
            result.Success.Should().BeTrue();
            var user = result.Value;

            user.DomainEvents.Should().ContainSingle(e => e is UserCreatedEvent);

            var userCreatedEvent = user.DomainEvents.OfType<UserCreatedEvent>().FirstOrDefault();
            userCreatedEvent.Should().NotBeNull();
            userCreatedEvent!.UserId.Should().Be(user.Id);
        }

        [Theory]
        [InlineData(VerificationCodeType.EmailVerification)]
        [InlineData(VerificationCodeType.PasswordReset)]
        public void GenerateVerificationCode_Should_Add_VerificationCode_And_DomainEvent(VerificationCodeType codeType)
        {
            // Arrange
            var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kjshsdi9?A"), false);

            // Act
            _userService.GenerateVerificationCode(user, codeType);

            // Assert
            user.VerificationCodes.Should().HaveCount(1);
            user.VerificationCodes.First().CodeType.Should().Be(codeType);
            user.DomainEvents.Should().ContainSingle(e => e is VerificationCodeGeneratedEvent);
        }
    }
}
