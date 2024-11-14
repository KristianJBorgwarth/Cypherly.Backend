using AutoMapper;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Create;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Common.Messaging.Messages.RequestMessages.User.Create;
using Cypherly.Domain.Common;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.CommandTest.CreateTest;

public class CreateUserCommandHandlerTests
{
    private readonly IUserRepository _fakeRepo;
    private readonly IUserLifeCycleServices _fakeUserLifeCycleServices;
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly IRequestClient<CreateUserProfileRequest> _fakeRequestClient;
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTests()
    {
        _fakeRepo = A.Fake<IUserRepository>();
        _fakeUserLifeCycleServices = A.Fake<IUserLifeCycleServices>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        _fakeRequestClient = A.Fake<IRequestClient<CreateUserProfileRequest>>();
        var fakeLogger = A.Fake<ILogger<CreateUserCommandHandler>>();


        _sut = new CreateUserCommandHandler(_fakeRepo, _fakeUserLifeCycleServices, _fakeUnitOfWork, _fakeRequestClient, fakeLogger);
    }

    [Fact]
    public async void Handle_Valid_Command_Should_Return_ResultOk()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "test@mail.dk",
            Password = "password923K=?",
            Username = "validUsername"

        };

        var user = new User(Guid.NewGuid(), Email.Create(cmd.Email), Password.Create(cmd.Password), isVerified: false);

        A.CallTo(() => _fakeRepo.GetByEmailAsync(cmd.Email)).Returns<User>(null);
        A.CallTo(() => _fakeUserLifeCycleServices.CreateUser(cmd.Email, cmd.Password)).Returns(Result.Ok(user));
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).DoesNothing();
        A.CallTo(() => _fakeRepo.CreateAsync(A<User>.Ignored)).DoesNothing();


        // Create the response message you want to return
        var responseMessage = new CreateUserProfileResponse();

        // Fake the MassTransit response
        var fakeMassTransitResponse = A.Fake<MassTransit.Response<CreateUserProfileResponse>>();
        A.CallTo(() => fakeMassTransitResponse.Message).Returns(responseMessage);

        // Simulate the response from requestClient for profile creation
        A.CallTo(() => _fakeRequestClient.GetResponse<CreateUserProfileResponse>(A<CreateUserProfileRequest>.Ignored, A<CancellationToken>.Ignored, A<RequestTimeout>.Ignored))
            .Returns(Task.FromResult(fakeMassTransitResponse));

        // Act
        var result = await _sut.Handle(cmd, new CancellationToken());

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Email.Should().Be(cmd.Email);

        A.CallTo(() => _fakeRepo.GetByEmailAsync(cmd.Email)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.CreateAsync(A<User>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();

        A.CallTo(() => _fakeRequestClient.GetResponse<CreateUserProfileResponse>(A<CreateUserProfileRequest>.Ignored, A<CancellationToken>.Ignored, A<RequestTimeout>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_Given_Email_Exists_Should_Return_ResultFail()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "test@mail.dk",
            Password = "password923K=?",
            Username = "validUsername"
        };


        var existingUser = new User(Guid.NewGuid(), Email.Create(cmd.Email), Password.Create(cmd.Password), isVerified: false);

        A.CallTo(() => _fakeRepo.GetByEmailAsync(cmd.Email)).Returns(existingUser);

        // Act
        var result = await _sut.Handle(cmd, new CancellationToken());

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("An account already exists with that email");

        A.CallTo(() => _fakeRepo.GetByEmailAsync(cmd.Email)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.CreateAsync(A<User>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeRequestClient.GetResponse<CreateUserProfileResponse>(A<CreateUserProfileRequest>.Ignored, A<CancellationToken>.Ignored, A<RequestTimeout>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async void Handle_Given_UserService_CreateUser_Fails_Should_Return_ResultFail()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "test@mail.dk",
            Password = "password923K=?",
            Username = "validUsername"
        };


        A.CallTo(() => _fakeRepo.GetByEmailAsync(cmd.Email)).Returns<User>(null);
        A.CallTo(() => _fakeUserLifeCycleServices.CreateUser(cmd.Email, cmd.Password)).Returns(Result.Fail<User>(Errors.General.UnspecifiedError("error")));

        // Act
        var result = await _sut.Handle(cmd, new CancellationToken());

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("error");

        A.CallTo(() => _fakeRepo.GetByEmailAsync(cmd.Email)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUserLifeCycleServices.CreateUser(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.CreateAsync(A<User>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeRequestClient.GetResponse<CreateUserProfileResponse>(A<CreateUserProfileRequest>.Ignored, A<CancellationToken>.Ignored, A<RequestTimeout>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async void Handle_Exception_Is_Thrown_Should_Return_ResultFail()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "test@mail.dk",
            Password = "password923K=?",
            Username = "validUsername"
        };


        // Simulate an exception when calling the repository's GetUserByEmail method
        A.CallTo(() => _fakeRepo.GetByEmailAsync(cmd.Email)).Throws(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(cmd, new CancellationToken());

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("Exception occured while attempting to create a user. Check logs for more information");

        // Verify the exception was thrown and handled
        A.CallTo(() => _fakeRepo.GetByEmailAsync(cmd.Email)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.CreateAsync(A<User>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeRequestClient.GetResponse<CreateUserProfileResponse>(A<CreateUserProfileRequest>.Ignored, A<CancellationToken>.Ignored, A<RequestTimeout>.Ignored))
            .MustNotHaveHappened();
    }
}
