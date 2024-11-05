using AutoMapper;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Claim.Commands.Create;
using Cypherly.Authentication.Domain.Services.Claim;
using Cypherly.Domain.Common;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.ClaimTest.CommandTest.CreateTest.Claim;

public class CreateClaimCommandHandlerTest
{
    private readonly IClaimRepository _fakeRepo;
    private readonly IClaimService _fakeService;
    private readonly IUnitOfWork _fakeUow;
    private readonly IMapper _fakeMapper;
    private readonly CreateClaimCommandHandler _sut;
    
    public CreateClaimCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IClaimRepository>();
        _fakeService = A.Fake<IClaimService>();
        _fakeUow = A.Fake<IUnitOfWork>();
         var fakeLogger = A.Fake<ILogger<CreateClaimCommandHandler>>();
        _fakeMapper = A.Fake<IMapper>();
        _sut = new(_fakeRepo, _fakeService, _fakeUow, fakeLogger, _fakeMapper);
    }

    [Fact]
    public async void Handle_Valid_Command_Should_Create_Claim()
    {
        // Arrange
        var command = new CreateClaimCommand()
        {
            ClaimType = "TestClaim"
        };
        
        A.CallTo(() => _fakeRepo.DoesClaimExistAsync(command.ClaimType)).Returns(false);
        A.CallTo(() => _fakeService.CreateClaim(command.ClaimType)).Returns(Result.Ok(new Domain.Aggregates.Claim(Guid.NewGuid(),"TestClaim")));
        A.CallTo(() => _fakeRepo.CreateAsync(A<Domain.Aggregates.Claim>._)).DoesNothing();
        A.CallTo(() => _fakeUow.SaveChangesAsync(CancellationToken.None)).DoesNothing();
        A.CallTo(() => _fakeMapper.Map<CreateClaimDto>(A<Domain.Aggregates.Claim>._)).Returns(new CreateClaimDto(){ClaimType = "TestClaim"});
        
        // Act
        var result = await _sut.Handle(command, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeTrue();
        result.Value.ClaimType.Should().Be("TestClaim");
        A.CallTo(() => _fakeRepo.DoesClaimExistAsync(command.ClaimType)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.CreateClaim(command.ClaimType)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.CreateAsync(A<Domain.Aggregates.Claim>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeMapper.Map<CreateClaimDto>(A<Domain.Aggregates.Claim>._)).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async void Handle_Claim_Already_Exists_Should_Return_Error()
    {
        // Arrange
        var command = new CreateClaimCommand()
        {
            ClaimType = "TestClaim"
        };
        
        A.CallTo(() => _fakeRepo.DoesClaimExistAsync(command.ClaimType)).Returns(true);
        
        // Act
        var result = await _sut.Handle(command, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("Claim already exists");
        A.CallTo(() => _fakeRepo.DoesClaimExistAsync(command.ClaimType)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.CreateClaim(command.ClaimType)).MustNotHaveHappened();
        A.CallTo(() => _fakeRepo.CreateAsync(A<Domain.Aggregates.Claim>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(CancellationToken.None)).MustNotHaveHappened();
        A.CallTo(() => _fakeMapper.Map<CreateClaimDto>(A<Domain.Aggregates.Claim>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async void Handle_Create_Claim_Fails_Should_Return_Error()
    {
        // Arrange
        var command = new CreateClaimCommand()
        {
            ClaimType = "TestClaim"
        };
        
        A.CallTo(() => _fakeRepo.DoesClaimExistAsync(command.ClaimType)).Returns(false);
        
        A.CallTo(() => _fakeService.CreateClaim(command.ClaimType)).Returns(Result.Fail<Domain.Aggregates.Claim>(Errors.General.UnspecifiedError("Error creating claim")));
        
        // Act
        var result = await _sut.Handle(command, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("Error creating claim");
        A.CallTo(() => _fakeRepo.DoesClaimExistAsync(command.ClaimType)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.CreateClaim(command.ClaimType)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.CreateAsync(A<Domain.Aggregates.Claim>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(CancellationToken.None)).MustNotHaveHappened();
        A.CallTo(() => _fakeMapper.Map<CreateClaimDto>(A<Domain.Aggregates.Claim>._)).MustNotHaveHappened();
    }
    
    [Fact]
    public async void Handle_Save_Changes_Throws_Exception_Should_Return_Error()
    {
        // Arrange
        var command = new CreateClaimCommand()
        {
            ClaimType = "TestClaim"
        };
        
        A.CallTo(() => _fakeRepo.DoesClaimExistAsync(command.ClaimType)).Returns(false);
        A.CallTo(() => _fakeService.CreateClaim(command.ClaimType)).Returns(Result.Ok(new Domain.Aggregates.Claim(Guid.NewGuid(),"TestClaim")));
        
        A.CallTo(() => _fakeRepo.CreateAsync(A<Domain.Aggregates.Claim>._)).DoesNothing();
        
        A.CallTo(() => _fakeUow.SaveChangesAsync(CancellationToken.None)).Throws<Exception>();
        
        // Act
        var result = await _sut.Handle(command, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("An exception occurred while creating the claim");
        A.CallTo(() => _fakeRepo.DoesClaimExistAsync(command.ClaimType)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.CreateClaim(command.ClaimType)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.CreateAsync(A<Domain.Aggregates.Claim>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeMapper.Map<CreateClaimDto>(A<Domain.Aggregates.Claim>._)).MustNotHaveHappened();
    }

}