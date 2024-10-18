using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;
using FluentAssertions;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetUserProfileByTag;

public class GetUserProfileByTagQueryValidatorTest
{
    private readonly GetUserProfileByTagQueryValidator _sut = new();

    [Fact]
    public void Validate_Given_Empty_Id_Should_Fail()
    {
        // Arrange
        var query = new GetUserProfileByTagQuery
        {
            Id = Guid.Empty,
            Tag = "tag"
        };
        
        //Act
        var result = _sut.Validate(query);
        
        //Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Given_Empty_Tag_Should_Fail()
    {
        // Arrange
        var query = new GetUserProfileByTagQuery
        {
            Id = Guid.NewGuid(),
            Tag = ""
        };
        
        //Act
        var result = _sut.Validate(query);
        
        //Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public void Validate_Given_Null_Tag_Should_Fail()
    {
        // Arrange
        var query = new GetUserProfileByTagQuery
        {
            Id = Guid.NewGuid(),
            Tag = null
        };
        
        //Act
        var result = _sut.Validate(query);
        
        //Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public void Validate_Given_Valid_Query_Should_Pass()
    {
        // Arrange
        var query = new GetUserProfileByTagQuery
        {
            Id = Guid.NewGuid(),
            Tag = "tag"
        };
        
        //Act
        var result = _sut.Validate(query);
        
        //Assert
        result.IsValid.Should().BeTrue();
    }
    
}