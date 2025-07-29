using BarberBookingAPI.Data;
using BarberBookingAPI.DTOs.Account;
using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Mapper;
using BarberBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberBookingAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly UserManager<ApplicationUser> _userManager;
        public AccountController(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUser = new ApplicationUser
                {
                    UserName = register.Username,
                    Email = register.Email
                };

                var createUser = await _userManager.CreateAsync(appUser, register.Password);

                if (!createUser.Succeeded)
                    return BadRequest(createUser.Errors); 

                //  Assign the "User" role to the newly created user
                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                if (!roleResult.Succeeded)
                    return StatusCode(500, roleResult.Errors);

                return Ok("User created and role assigned!");
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }
    }
}
