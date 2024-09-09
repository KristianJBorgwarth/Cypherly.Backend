using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;
using FluentAssertions;
using Xunit;

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
            string email = "test@mail.com";
            string invalidPassword = "short"; // Assuming password should meet certain criteria

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
            string email = "test@mail.com";
            string password = "Password123!";

            // Act
            var result = _userService.CreateUser(email, password);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Email.Address.Should().Be(email);
            result.Value.IsVerified.Should().BeFalse();
            result.Value.VerificationCode.Should().NotBeNull();
        }

        [Fact]
        public void CreateUser_Should_Add_UserCreatedEvent()
        {
            // Arrange
            string email = "test@mail.com";
            string password = "Password123!";

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

        [Fact]
        public void CreateUser_Should_Set_VerificationCode_When_User_Is_Created()
        {
            // Arrange
            string email = "test@mail.com";
            string password = "Password123!";

            // Act
            var result = _userService.CreateUser(email, password);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.VerificationCode.Should().NotBeNull();
            result.Value.VerificationCode.Code.Should().NotBeNullOrWhiteSpace();
        }
    }
}
