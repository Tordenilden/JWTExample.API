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
    /// <summary>
    /// vi vil gerne DI vores database / repo herind
    /// DI vores JWTSettings....
    /// 
    /// oprette en metode der spørger db om u + p
    /// hvis bruger findes... opret token og returner token i utf8 format
    /// Det der er vigtigt at forstå er Claims!!
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersControlleOldr : ControllerBase
    {
        #region start
        private readonly AbContext context;
        private readonly JWTSetting setting; //.json fil

        // JWTSetting den skal laves om til IOption
        public UsersControlleOldr(AbContext context, IOptions<JWTSetting> setting)
        {
            this.context = context;
            this.setting = setting.Value;
        }
        #endregion start

        // vores metode til at teste vores u + p
        [HttpPost("authenticate")]
        public IActionResult authenticate(Users account)
        {
            // find bruger
            var user = context.Users
            .FirstOrDefault(u => u.username == account.username &&
            u.password == account.password);
            // hvis jeg er logget forkert ind?
            if (user == null) return Unauthorized();//401
            var tokenHandler = new JwtSecurityTokenHandler();// read/ validates JSON web token, i forskellige formater
            // transform UTF16 => UTF8
            var tokenKey = System.Text.Encoding.UTF8.GetBytes(this.setting.secretkey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.username),
                        new Claim(ClaimTypes.Role, user.role),
                        //new Claim(ClaimTypes.) kigger nærmere på...

                    }
                ),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256)
            };
            // så skal vores token "oprettes"
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var finalToken = tokenHandler.WriteToken(token);

            return Ok(finalToken);
        }
    }
}
