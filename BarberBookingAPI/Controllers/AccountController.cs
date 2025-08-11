using BarberBookingAPI.Data;
using BarberBookingAPI.DTOs.Account;
using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Mapper;
using BarberBookingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberBookingAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService; 
        public AccountController(UserManager<ApplicationUser> userManager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailService = emailService;
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

                // Assign the "User" role to the newly created user
                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                if (!roleResult.Succeeded)
                    return StatusCode(500, roleResult.Errors);

                // Send confirmation email
                await _emailService.SendEmailAsync(
                    appUser.Email,
                    "Welcome to BarberBooking!",
                    $"Dear {appUser.UserName}, your registration was successful. You can now book your appointment with ease!"
                );

                return Ok(
                    new NewUserDto
                    {
                        UserName = appUser.UserName,
                        Email = appUser.Email,
                        Token = await _tokenService.CreateTokenAsync(appUser)
                    }
                );
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == login.Username.ToLower());

                if (user == null)
                    return Unauthorized(new { error = "Invalid email or password" });

                var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);

                if (!result.Succeeded)
                    return Unauthorized(new { error = "Invalid email or password" });

                return Ok(
                    new NewUserDto
                    { 
                        UserName = user.UserName,
                        Email = user.Email,
                        Token = await _tokenService.CreateTokenAsync(user)
                    }
                 );

            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }
        [HttpGet("workers")]
        [Authorize] 
        public async Task<IActionResult> GetAllWorkers()
        {
            var workers = await _userManager.GetUsersInRoleAsync("Worker");

            
            var workersDto = workers.Select(w => new
            {
                w.FullName
            }).ToList();

            return Ok(workersDto);
        }
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(assignRoleDto.Username);
                if (user == null)
                    return NotFound("User not found");

                // Remove user from all roles if needed
                var currentRoles = await _userManager.GetRolesAsync(user);
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return StatusCode(500, removeResult.Errors);

                // Assing new role
                var addResult = await _userManager.AddToRoleAsync(user, assignRoleDto.Role);
                if (!addResult.Succeeded)
                    return StatusCode(500, addResult.Errors);

                return Ok($"Role '{assignRoleDto.Role}' assigned to user '{assignRoleDto.Username}'.");
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null) return NotFound("User not found");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password has been reset.");
        }

    }
}
