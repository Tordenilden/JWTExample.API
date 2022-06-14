using JWTExample.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JWTExample.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // marker properties og alt+enter => giver ctor
        private readonly AbContext context;
        private readonly JWTSetting setting;

        //refreshtoken 
        // private readonly IRefreshTokenGenerator tokenGenerator;

        public UsersController(AbContext context, IOptions<JWTSetting> setting)
        {
            this.context = context;
            this.setting = setting.Value;
            //this.tokenGenerator = g;
        }

        //Refreshtoken
        //[NonAction]
        //public TokenResponse Authenticate(string username, Claim[] claims)
        //{
        //    TokenResponse tokenResponse = new TokenResponse();
        //    var tokenkey = Encoding.UTF8.GetBytes(setting.secretkey);
        //    var tokenhandler = new JwtSecurityToken(
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(15),
        //         signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)

        //        );
        //    tokenResponse.JWTToken = new JwtSecurityTokenHandler().WriteToken(tokenhandler);
        //    tokenResponse.refreshToken = tokenGenerator.generateToken(username);

        //    return tokenResponse;
        //}


        /// <summary>
        /// tjek om credientials matcher
        /// Hvis de matcher create token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] UserCredentials user)
        {
            //refreshtoken
           // TokenResponse tokenResponse = new TokenResponse();

            // version 1
            var _user = context.Users.FirstOrDefault(o => o.username == user.username && o.password == user.password);
            if (_user == null)
                return Unauthorized();

            var tokenhandler = new JwtSecurityTokenHandler();//Reads and validates a 'JSON Web Token' (JWT) 
            var tokenkey = System.Text.Encoding.UTF8.GetBytes(setting.secretkey); // unicode characters => sequence of bytes, 16 bit til 8 bit system
            var tokenDescriptor = new SecurityTokenDescriptor // Represents the cryptographic key and security algorithms that are used to generate a digital signature.
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, _user.username),
                        new Claim(ClaimTypes.Role, _user.role)

                    }
                ),
                Expires = DateTime.Now.AddSeconds(600),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            var finalToken = tokenhandler.WriteToken(token);
            return Ok(finalToken);

            //refreshToken
            //tokenResponse.JWTToken = tokenhandler.WriteToken(token);
            //tokenResponse.refreshToken = tokenGenerator.generateToken(user.username);
            //return Ok(tokenResponse);
        }


        /// <summary>
        /// setting.secretkey er fra vores config fil derfor vi har lavet vores class....
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        //[Route("Refresh")]
        //[HttpPost]
        //public IActionResult Refresh([FromBody] TokenResponse token)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    SecurityToken securityToken1;
        //    var principal = tokenHandler.ValidateToken(token.JWTToken, new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.secretkey)),
        //        ValidateIssuer = false,
        //        ValidateAudience = false

        //    }, out securityToken1);

        //    var _token = securityToken1 as JwtSecurityToken;
        //    if (_token != null && !_token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
        //    {
        //        return Unauthorized();
        //    }
        //    var username = principal.Identity.Name;
        //    var _reftable = context.TblRefreshToken.FirstOrDefault(o => o.username == username && o.refreshToken == token.refreshToken);
        //    if (_reftable == null)
        //    {
        //        return Unauthorized();
        //    }
        //    TokenResponse _result = Authenticate(username, principal.Claims.ToArray());


        //    //var tokenHandler = new JwtSecurityTokenHandler();
        //    //var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token.JWTToken);
        //    //var username = securityToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;


        //    ////var username = principal.Identity.Name;
        //    //var _reftable = context.TblRefreshToken.FirstOrDefault(o => o.username == username && o.refreshToken == token.refreshToken);
        //    //if (_reftable == null)
        //    //{
        //    //    return Unauthorized();
        //    //}
        //    //TokenResponse _result = Authenticate(username, securityToken.Claims.ToArray());
        //    return Ok(_result);
        //}


    }
}
