using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public class GetUserProfileByTagQueryHandler(
    IUserProfileRepository userProfileRepository,
    ILogger<GetUserProfileByTagDto> logger,
    IUserProfileService userProfileService)
    : IQueryHandler<GetUserProfileByTagQuery, GetUserProfileByTagDto>
{
    public Task<Result<GetUserProfileByTagDto>> Handle(GetUserProfileByTagQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}