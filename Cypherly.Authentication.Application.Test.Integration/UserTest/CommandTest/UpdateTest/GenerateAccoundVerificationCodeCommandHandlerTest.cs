using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.CommandTest.UpdateTest;

public class GenerateAccoundVerificationCodeCommandHandlerTest : IntegrationTestBase
{
    private readonly GenerateAccountVerificationCodeCommandHandler _sut;
    public GenerateAccoundVerificationCodeCommandHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserLifeCycleServices>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GenerateAccountVerificationCodeCommandHandler>>();
        _sut = new(repo, unitOfWork, userService, logger);
    }

    [Fact]
    public async Task Handle_Valid_Command_Should_Generate_New_Verification_Code()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kj9823HHj?"), false);

        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var cmd = new GenerateAccountVerificationCodeCommand
        {
            UserId = user.Id
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        Db.User.AsNoTracking().First().VerificationCodes.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Command_With_Invalid_Id_Should_Return_Result_Fail()
    {
        // Arrange
        var cmd = new GenerateAccountVerificationCodeCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async Task Handle_User_With_Verified_Status_Should_Return_Result_Fail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kj9823HHj?"),
            true); // Verified status set to true

        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var cmd = new GenerateAccountVerificationCodeCommand
        {
            UserId = user.Id
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("User is already verified");
    }
}