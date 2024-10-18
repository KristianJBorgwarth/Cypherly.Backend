using AutoMapper;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.QueryTest.GetUserProfileByTagTest;

public class GetUserProfileByTagQueryHandlerTest : IntegrationTestBase
{
    private readonly GetUserProfileByTagQueryHandler _sut;
    public GetUserProfileByTagQueryHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetUserProfileByTagQueryHandler>>();
        var profilePictureService = scope.ServiceProvider.GetRequiredService<IProfilePictureService>();
        var userProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileService>();
        
        _sut = new(repo, userProfileService, profilePictureService, mapper, logger);
    }

    [Fact]
    public async Task Handle_Valid_Query_Should_Return_UserProfile()
    {
        // Arrange
        var requestingUser = new UserProfile(Guid.NewGuid(), "requestingUser", UserTag.Create("requestingUser"));
        var userProfile = new UserProfile(Guid.NewGuid(), "userProfile", UserTag.Create("userProfile"));
        await Db.AddRangeAsync(requestingUser, userProfile);
        await Db.SaveChangesAsync();
        
        var query = new GetUserProfileByTagQuery() {Id = requestingUser.Id, Tag = userProfile.UserTag.Tag};
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Username.Should().Be(userProfile.Username);
        result.Value.UserTag.Should().Be(userProfile.UserTag.Tag);
    }
    
    [Fact]
    public async Task Handle_Query_When_User_Does_Not_Exist_Should_Return_NotFound()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "userProfile", UserTag.Create("userProfile"));
        await Db.AddAsync(userProfile);
        await Db.SaveChangesAsync();
        
        var query = new GetUserProfileByTagQuery() {Id = Guid.NewGuid(), Tag = userProfile.UserTag.Tag};
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Contain("entity.not.found");
    }
    
    [Fact]
    public async Task Handle_Query_When_UserProfile_Does_Not_Exist_Should_Return_EmptyDto()
    {
        // Arrange
        var requestingUser = new UserProfile(Guid.NewGuid(), "requestingUser", UserTag.Create("requestingUser"));
        await Db.AddAsync(requestingUser);
        await Db.SaveChangesAsync();
        
        var query = new GetUserProfileByTagQuery() {Id = requestingUser.Id, Tag = "userProfile"};
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(null);
    }
    
    [Fact]
    public async Task Handle_Query_When_User_Is_Blocked_Should_Return_EmptyDto()
    {
        var requestingUser = new UserProfile(Guid.NewGuid(), "requestingUser", UserTag.Create("requestingUser"));
        var userProfile = new UserProfile(Guid.NewGuid(), "userProfile", UserTag.Create("userProfile"));
        requestingUser.BlockUser(userProfile.Id);
        await Db.AddRangeAsync(requestingUser, userProfile);
        await Db.SaveChangesAsync();
        
        var query = new GetUserProfileByTagQuery() {Id = requestingUser.Id, Tag = userProfile.UserTag.Tag};
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(null);
    }
    
}