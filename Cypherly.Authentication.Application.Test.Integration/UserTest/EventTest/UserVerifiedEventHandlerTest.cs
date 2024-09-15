using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Events;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.EventTest;

public class UserVerifiedEventHandlerTest : IntegrationTestBase
{
    private readonly UserVerifiedEventHandler _sut;
    public UserVerifiedEventHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var claimRepository = scope.ServiceProvider.GetRequiredService<IClaimRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserVerifiedEventHandler>>();

        _sut = new(userRepository, claimRepository, unitOfWork, logger);
    }

    [Fact]
    public async Task Handle_Valid_Notification_Should_Create_UserClaim()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("aæskjd=??232KF") , false);
        var claim = new Claim(Guid.NewGuid(), "user");
        await Db.Claim.AddAsync(claim);
        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var notification = new UserVerifiedEvent(user.Id);

        // Act
        await _sut.Handle(notification, default);

        // Assert
        Db.UserClaim.Should().HaveCount(1);
        Db.UserClaim.AsNoTracking().FirstOrDefault(uc => uc.UserId == user.Id)!.ClaimId.Should().Be(claim.Id);
    }

    [Fact]
    public async Task Handle_Notification_Invalid_Id_Should_Not_Create_UserClaim()
    {
        // Arrange
        var notification = new UserVerifiedEvent(Guid.NewGuid());

        // Act
        await _sut.Handle(notification, default);

        // Assert
        Db.UserClaim.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Notification_Invalid_Claim_Should_Not_Create_UserClaim()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("aæskjd=??232KF") , false);
        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var notification = new UserVerifiedEvent(user.Id);

        // Act
        await _sut.Handle(notification, default);

        // Assert
        Db.UserClaim.Should().BeEmpty();
    }
}