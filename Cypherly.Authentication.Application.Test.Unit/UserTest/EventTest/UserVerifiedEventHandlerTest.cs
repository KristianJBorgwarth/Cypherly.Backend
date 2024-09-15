using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Events;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.EventTest;

public class UserVerifiedEventHandlerTest
{
    private readonly IUserRepository _fakeUserRepository;
    private readonly IClaimRepository _fakeClaimRepository;
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly UserVerifiedEventHandler _sut;

    public UserVerifiedEventHandlerTest()
    {
        _fakeUserRepository = A.Fake<IUserRepository>();
        _fakeClaimRepository = A.Fake<IClaimRepository>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        var fakeLogger = A.Fake<ILogger<UserVerifiedEventHandler>>();

        _sut = new(_fakeUserRepository, _fakeClaimRepository, _fakeUnitOfWork, fakeLogger);
    }

    [Fact]
    public async Task Handle_Valid_Notification_Should_Create_UserClaim()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("sdaASDASdd?23"), false);

        var claim = new Claim(Guid.NewGuid(), "user");
        A.CallTo(()=> _fakeUserRepository.GetByIdAsync(user.Id)).Returns(user);
        A.CallTo(() => _fakeClaimRepository.GetClaimByTypeAsync("user", default)).Returns(claim);

        var notification = new UserVerifiedEvent(user.Id);

        // Act
        await _sut.Handle(notification, default);

        // Assert
        A.CallTo(() => _fakeUserRepository.GetByIdAsync(user.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeClaimRepository.GetClaimByTypeAsync("user", default)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeClaimRepository.UpdateAsync(claim)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(default)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Notification_Invalid_Id_Should_Not_Create_UserClaim()
    {
        // Arrange
        var notification = new UserVerifiedEvent(Guid.NewGuid());
        A.CallTo(()=> _fakeUserRepository.GetByIdAsync(notification.UserId)).Returns<User?>(null);

        // Act
        await _sut.Handle(notification, default);

        // Assert
        A.CallTo(() => _fakeUserRepository.GetByIdAsync(notification.UserId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeClaimRepository.GetClaimByTypeAsync("user", default)).MustNotHaveHappened();
        A.CallTo(() => _fakeClaimRepository.UpdateAsync(A<Claim>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Claim_Not_Found_Should_Not_Create_UserClaim()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("sdaASDASdd?23"), false);

        A.CallTo(()=> _fakeUserRepository.GetByIdAsync(user.Id)).Returns(user);
        A.CallTo(() => _fakeClaimRepository.GetClaimByTypeAsync("user", default)).Returns<Claim?>(null);

        var notification = new UserVerifiedEvent(user.Id);

        // Act
        await _sut.Handle(notification, default);

        // Assert
        A.CallTo(() => _fakeUserRepository.GetByIdAsync(user.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeClaimRepository.GetClaimByTypeAsync("user", default)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeClaimRepository.UpdateAsync(A<Claim>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
    }
}