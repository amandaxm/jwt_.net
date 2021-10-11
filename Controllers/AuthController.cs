using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ultimoteste.Data;
using ultimoteste.Dtos;
using ultimoteste.Models;
using Org.BouncyCastle.Crypto.Generators;
using ultimoteste.Helpers;
using System;

namespace ultimoteste.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly JwtService jwtService;

        public AuthController(IUserRepository userRepository, JwtService jwtService)
        {
            this.userRepository = userRepository;
            this.jwtService = jwtService;
        }

        [HttpPost("register")]
        public IActionResult register(RegisterDto registerDto)
        {

            var user = new User {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };
            userRepository.Create(user);
            return Created("success", userRepository.Create(user));
        }

        [HttpPost("login")]
        public IActionResult login(LoginDto loginDto) {
            var user = userRepository.GetByEmail(loginDto.Email);
            if (user == null)
                return BadRequest(new { message = "Invalid credentials" });
            //se nao for igual
            if(!BCrypt.Net.BCrypt.Verify(loginDto.Password,user.Password))
                return BadRequest(new { message = "Invalid credentials" });


            var jwt = jwtService.Generate(user.Id);

            Response.Cookies.Append("jwt", jwt, new CookieOptions { HttpOnly = true });
            return Ok(new { message = "success"});



        }

        [HttpGet("user")]
        public IActionResult User() {
            try
            {
                var jwt = Request.Cookies["jwt"];
                var token = jwtService.Verify(jwt);
                int userId = int.Parse(token.Issuer);
                var user = userRepository.GetById(userId);
                return Ok(user);
            }
            catch (Exception e) {
                return Unauthorized();
            }
        }
        [HttpPost("logout")]
        public IActionResult Logout() {
            Response.Cookies.Delete("jwt");
            return Ok(new
            {
                message = "success"
            });
        
        }
    }
}
