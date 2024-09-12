using AutoMapper;
using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileById;

public class GetUserProfileByIdQueryHandler(
    IUserProfileRepository userProfileRepository,
    IMapper mapper,
    ILogger<GetUserProfileByIdQueryHandler> logger)
    : IQueryHandler<GetUserProfileByIdQuery, GetUserProfileByIdDto>
{
    public async Task<Result<GetUserProfileByIdDto>> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userprofile = await userProfileRepository.GetByIdAsync(request.UserProfileId);
            if (userprofile is null)
                return Result.Fail<GetUserProfileByIdDto>(Errors.General.NotFound(request.UserProfileId.ToString()));

            var dto = mapper.Map<GetUserProfileByIdDto>(userprofile);
            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occured while handling request with id {Id}", request.UserProfileId);
            return Result.Fail<GetUserProfileByIdDto>(Errors.General.UnspecifiedError("An exception occured while handling the request"));
        }
    }
}