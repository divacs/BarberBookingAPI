using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BarberBookingAPI.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }
        public async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {                
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty), // Email claim for the user
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),  // Username claim for the user

            };
            // adding roles clame to the jwt token
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            // Add a unique token identifier (JTI) to distinguish this token instance and support potential token revocation or tracking
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Create a new instance of JwtSecurityTokenHandler to generate the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // ClaimsIdentity containing user claims
                Expires = DateTime.Now.AddDays(7), // Token expiration time
                SigningCredentials = creds,  // Signing credentials using the symmetric key
                Issuer = _config["JWT:Issuer"], // Issuer of the token
                Audience = _config["JWT:Audience"] // Audience for which the token is intended
            };

            // Create the token using JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler(); 
            var token = tokenHandler.CreateToken(tokenDescriptor); // Create the token based on the descriptor

            return tokenHandler.WriteToken(token); // Write the token to a string and return it
        }

    }
}
