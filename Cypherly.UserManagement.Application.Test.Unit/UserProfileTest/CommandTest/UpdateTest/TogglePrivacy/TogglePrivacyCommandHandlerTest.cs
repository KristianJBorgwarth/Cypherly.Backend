using Cypherly.Application.Contracts.Repository;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.TogglePrivacy;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.UnblockUser;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.UpdateTest.TogglePrivacy;

public class TogglePrivacyCommandHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IUnitOfWork _fakeUow;
    private readonly TogglePrivacyCommandHandler _sut;

    public TogglePrivacyCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeUow = A.Fake<IUnitOfWork>();
        var fakeLogger = A.Fake<ILogger<TogglePrivacyCommandHandler>>();
        _sut = new(_fakeRepo, _fakeUow, fakeLogger);
    }

    [Fact]
    public async Task TogglePrivacy_Command_Should_Update_Privacy()
    {
        // Arrange
        var profile = new UserProfile(Guid.NewGuid(), "ValidUsername", UserTag.Create("ValidUsername"));
        var command = new TogglePrivacyCommand
        {
            Id = profile.Id,
            IsPrivate = true
        };
        
        A.CallTo(()=>_fakeRepo.GetByIdAsync(command.Id)).Returns(profile);
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>.Ignored)).DoesNothing();
        // Act
        var result = await _sut.Handle(command, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeTrue();
        profile.IsPrivate.Should().BeTrue();
        A.CallTo(()=>_fakeRepo.GetByIdAsync(command.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task TogglePrivacy_UserNotFound_Should_Not_Update_Privacy()
    {
        var command = new TogglePrivacyCommand
        {
            Id = Guid.NewGuid(),
            IsPrivate = true
        };
        
        A.CallTo(()=>_fakeRepo.GetByIdAsync(command.Id)).Returns<UserProfile>(null);
        // Act
        var result = await _sut.Handle(command, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeFalse();
        A.CallTo(()=>_fakeRepo.GetByIdAsync(command.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }
}