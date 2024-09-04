using System.Security.AccessControl;
using AutoMapper;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Create;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.CommandTest.CreateTest;

public class CreateUserCommandHandlerTests
{
    private readonly IUserRepository _fakeRepo;
    private readonly IMapper _fakeMapper;
    private readonly IUserService _fakeUserService;
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly CreateUserCommandHandler _sut;
    
    public CreateUserCommandHandlerTests()
    {
        _fakeRepo = A.Fake<IUserRepository>();
        _fakeMapper = A.Fake<IMapper>();
        _fakeUserService = A.Fake<IUserService>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        var fakeLogger = A.Fake<ILogger<CreateUserCommandHandler>>();
        _sut = new(_fakeRepo, _fakeMapper, _fakeUserService, _fakeUnitOfWork, fakeLogger);
    }
    
    [Fact]
    public async void Handle_Valid_Command_Should_Return_ResultOk()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "test@mail.dk",
            Password = "password923K=?"
        };
        
        A.CallTo(()=> _fakeRepo.GetUserByEmail(cmd.Email)).Returns<User>(null);
        A.CallTo(()=> _fakeUserService.CreateUser(cmd.Email, cmd.Password)).Returns(Result.Ok(new User(Email.Create(cmd.Email), Password.Create(cmd.Password), isVerified:false)));
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).DoesNothing();
        A.CallTo(()=> _fakeRepo.CreateAsync(A<User>.Ignored)).DoesNothing();
        A.CallTo(()=> _fakeMapper.Map<CreateUserDto>(A<User>.Ignored)).Returns(new CreateUserDto
        {
            Email = cmd.Email,
            Id = default
        });
        
        // Act
        var result = await _sut.Handle(cmd, new CancellationToken());
        
        // Assert
        result.Success.Should().BeTrue();
        result.Value.Email.Should().Be(cmd.Email);
        A.CallTo(() => _fakeRepo.GetUserByEmail(cmd.Email)).MustHaveHappenedOnceExactly();
        A.CallTo(()=>_fakeMapper.Map<CreateUserDto>(A<User>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(()=>_fakeRepo.CreateAsync(A<User>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(()=>_fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_Given_Email_Exists_Should_Return_ResultFail()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "test@mail.dk",
            Password = "password923K=?"
        };
        
        A.CallTo(()=> _fakeRepo.GetUserByEmail(cmd.Email)).Returns(new User(Email.Create(cmd.Email), Password.Create(cmd.Password), isVerified:false));
        
        // Act
        var result = await _sut.Handle(cmd, new CancellationToken());
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("An account already exists with that email");
        A.CallTo(() => _fakeRepo.GetUserByEmail(cmd.Email)).MustHaveHappenedOnceExactly();
        A.CallTo(()=>_fakeMapper.Map<CreateUserDto>(A<User>.Ignored)).MustNotHaveHappened();
        A.CallTo(()=>_fakeRepo.CreateAsync(A<User>.Ignored)).MustNotHaveHappened();
        A.CallTo(()=>_fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(()=>_fakeUserService.CreateUser(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
    }
    
    [Fact]
    public async void Handle_Given_UserService_CreateUser_Fails_Should_Return_ResultFail()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "test@mail.dk",
            Password = "password923K=?"
        };
        
        A.CallTo(()=> _fakeRepo.GetUserByEmail(cmd.Email)).Returns<User>(null);
        A.CallTo(()=> _fakeUserService.CreateUser(cmd.Email, cmd.Password)).Returns(Result.Fail<User>(Errors.General.UnspecifiedError("error")));
        
        // Act
        var result = await _sut.Handle(cmd, new CancellationToken());
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("error");
        A.CallTo(() => _fakeRepo.GetUserByEmail(cmd.Email)).MustHaveHappenedOnceExactly();
        A.CallTo(()=>_fakeUserService.CreateUser(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(()=>_fakeMapper.Map<CreateUserDto>(A<User>.Ignored)).MustNotHaveHappened();
        A.CallTo(()=>_fakeRepo.CreateAsync(A<User>.Ignored)).MustNotHaveHappened();
        A.CallTo(()=>_fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }
    
    [Fact]
    public async void Handle_Exception_Is_Thrown_Should_Return_ResultFail()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "test@mail.dk",
            Password = "password923K=?"
        };

        // Simulate an exception when calling the repository's GetUserByEmail method
        A.CallTo(() => _fakeRepo.GetUserByEmail(cmd.Email)).Throws(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(cmd, new CancellationToken());

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("Exception occured while attempting to create a user. Check logs for more information");

        // Verify the exception was thrown and handled
        A.CallTo(() => _fakeRepo.GetUserByEmail(cmd.Email)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeMapper.Map<CreateUserDto>(A<User>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeRepo.CreateAsync(A<User>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }

    
    
    
        
}