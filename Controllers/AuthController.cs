using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomJwt.Attributes;
using CustomJwt.Constants;
using CustomJwt.Exceptions;
using CustomJwt.Repositories;
using CustomJwt.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CustomJwt.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly DataContext _db;

    public AuthController(DataContext db)
    {
        _db = db;
    }

    [AllowAnonymous]
    [HttpPost]
    public ActionResult Login([FromBody] LoginRequest request)
    {
        var user = _db.User
            .FirstOrDefault(x => x.Username == request.Username
                                 && x.Password == request.Password);

        if (user == null)
            throw new BadRequestException("User not found.");

        var accessToken = GenerateAccessToken(user.Id, user.RoleId.GetValueOrDefault());

        var response = new LoginResponse(accessToken, user.Id, user.RoleId, user.Username);
        
        return Ok(response);
    }
    
    private string GenerateAccessToken(int userId, int roleId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConst.Secret));
        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = JwtConst.Issuer,
            Audience = JwtConst.Audience,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtConst.UserId, userId.ToString()),
                new Claim(JwtConst.RoleId, roleId.ToString())
            }),
            Expires = DateTime.Now.AddMinutes(JwtConst.ExpiryMinutes),
            SigningCredentials = credential
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}