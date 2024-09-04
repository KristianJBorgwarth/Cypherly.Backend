using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cypherly.UserManagement.API.Controllers;

[Route("api/[controller]")]
public class UserProfileController(ISender sender) : BaseController
{

}