using AutoMapper;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Claim.Commands.Create.Claim;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Services.Claim;
using Cypherly.Authentication.Persistence.Context;
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
        
    }
}