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
            //var redirectUrl = Url.Action("GoogleResponse", "GoogleAuth");
            var redirectUrl = "http://localhost:5246/auth/google-response";
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Endpoint called by Google after successful login
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
                return BadRequest("Google authentication failed.");

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
