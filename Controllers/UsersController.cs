using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Medals_API.Authentication;
using System.Threading.Tasks;
using System;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Medals_API.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    // access UserManager w/ dependecy injection
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    public UsersController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
      _userManager = userManager;
      _configuration = configuration;
    }

    [HttpPost]
    [Route("login")]
    [SwaggerOperation(summary: "User login", null)]
    [SwaggerResponse(200, "Success", typeof(UserTokenDTO))]
    public async Task<IActionResult> Login([FromBody] UserLogin model)
    {
      var user = await _userManager.FindByNameAsync(model.Username);
      if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
      {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
          new Claim(JwtRegisteredClaimNames.Email, user.Email),
          new Claim(JwtRegisteredClaimNames.Jti, user.Id),
          new Claim("username", user.UserName),
        };

        foreach (var userRole in userRoles)
        {
          authClaims.Add(new Claim("roles", userRole));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
          issuer: _configuration["JWT:ValidIssuer"],
          audience: _configuration["JWT:ValidAudience"],
          expires: DateTime.Now.AddHours(3),
          claims: authClaims,
          signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return Ok(new UserTokenDTO
        {
          Token = new JwtSecurityTokenHandler().WriteToken(token),
          Expiration = Convert.ToString(token.ValidTo)
        });
      }
      return Unauthorized(new { Message = "Login falied" });
    }
  }
}
