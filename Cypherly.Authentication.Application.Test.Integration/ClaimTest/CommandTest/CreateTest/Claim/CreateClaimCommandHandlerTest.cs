using AutoMapper;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Claim.Commands.Create.Claim;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Services.Claim;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.ClaimTest.CommandTest.CreateTest.Claim;

public class CreateClaimCommandHandlerTest : IntegrationTestBase
{
    private readonly CreateClaimCommandHandler _sut;
    public CreateClaimCommandHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var claimRepository = scope.ServiceProvider.GetRequiredService<IClaimRepository>();
        var claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CreateClaimCommandHandler>>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

         _sut = new CreateClaimCommandHandler(claimRepository, claimService, unitOfWork, logger, mapper);
    }

    [Fact]
    public async void Handle_Valid_Command_Should_Create_Claim_And_Return_Result_Ok()
    {
        // Arrange
        var command = new CreateClaimCommand()
        {
            ClaimType = "testclaim"
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ClaimType.Should().Be(command.ClaimType);
        Db.Claim.Should().HaveCount(1);
    }

    [Fact]
    public async void Handle_Command_With_Existing_Claim_Should_Return_Result_Fail()
    {
        // Arrange
        var command = new CreateClaimCommand()
        {
            ClaimType = "admin"
        };
        await Db.Claim.AddAsync(new Domain.Aggregates.Claim(Guid.NewGuid(), "admin") );
        await Db.SaveChangesAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Claim already exists");
        Db.Claim.Should().HaveCount(1);
    }

    [Fact]
    public async void Handle_Command_When_ClaimType_Invalid_Should_Return_Result_Fail()
    {
        // Arrange
        var command = new CreateClaimCommand()
        {
            ClaimType = new string('a', 101) // ClaimType is too long
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Value 'claimType' should not exceed 30.");
        Db.Claim.Should().HaveCount(0);
    }
}