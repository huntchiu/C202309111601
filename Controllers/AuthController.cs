using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace C202309111601.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController: ControllerBase
{
    private readonly IConfiguration configuration;

    public AuthController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    
    [HttpPost]
    public IActionResult Authenticate([FromBody]Credential credential)
    {
        // Verify the credential
        if (credential.UserName == "admin" && credential.Password == "123456")
        {
            // Creating the security context
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                new Claim("Admin", "true")
            }; 

            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            return Ok(new
            {
                access_token = CreateToken(claims,expiresAt),
                expires_at = expiresAt
            });
                
        }

        ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint.");
        return Unauthorized(ModelState);
    }
    

    private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
    {
        var secretKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey")??"");

        // generate the JWT
        // notBefore: 令牌在此時間之前不可用。
        // expires: 令牌的到期時間。
        // new SymmetricSecurityKey(secretKey)： 對稱密鑰
        var jwt = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.UtcNow,    
            expires: expireAt,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature)
        );
        // JwtSecurityToken 是繼承 SecurityToken 的Class 
        // 使用JwtSecurityTokenHandler 的WriteToken來將生成的jwt物件轉換為字符串形式
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

}


public class Credential
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}