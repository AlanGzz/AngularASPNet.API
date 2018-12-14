using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AngularASPNet.API.Data;
using AngularASPNet.API.Dtos;
using AngularASPNet.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AngularASPNet.API.Controllers 
{
    [Route("api/[controller]")]

    //Esto nos da controles de validaciones, etc
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;

        public IConfiguration config { get; }

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            this.repo = repo;
            this.config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult>Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            //Consulta en la base de datos por el usuario
            if(await repo.UserExists(userForRegisterDto.Username))
                //Si ya hay un usuario con el nombre, retorna un badrequest
                return BadRequest("This username is already taken.");

            //Si no hay un Usuario con el nombre, crea uno
            var userToCreate  = new Users{
                UserName = userForRegisterDto.Username
            };

            //Llama al metodo register para crear un usuario
            var createdUser = await this.repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await this.repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if(userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userForLoginDto.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}