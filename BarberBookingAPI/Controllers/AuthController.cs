using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        // Endpoint to redirect to Google's login page
        [HttpGet("login")]
        public IActionResult LoginWithGoogle()
        {
            // This will redirect the user to Google's OAuth 2.0 server for authentication
            // var redirectUrl = "http://localhost:5246/api/GoogleAuth/google-response";
            var redirectUrl = Url.Action("GoogleResponse", "GoogleAuth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Endpoint called by Google after successful login
        [AllowAnonymous]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                var failure = result.Failure?.Message ?? "Unknown failure";
                return BadRequest($"Google authentication failed: {failure}");
            }

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims.Select(claim => new
                {
                    claim.Type,
                    claim.Value
                });

            return Ok(new
            {
                Message = "Google login successful!",
                Claims = claims
            });
        }
    }
}
