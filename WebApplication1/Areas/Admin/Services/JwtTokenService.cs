using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using WebApplication1.Areas.Identity;

namespace WebApplication1.Areas.Admin.Services
{
    public static class JwtTokenService
    {

        private static readonly string Secret = ConfigurationManager.AppSettings["Jwt:Secret"]; // 32+ char random string, in Web.config
        private static readonly string Issuer = "SwiftFinancialsApi";

        public static string GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

            // Assumes ApplicationUser.EmployeeId exists (per the commented Register code) — confirm this
            if (user.EmployeeId != Guid.Empty)
                claims.Add(new Claim("EmployeeId", user.EmployeeId.ToString()));

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer, audience: Issuer, claims: claims,
                expires: DateTime.UtcNow.AddHours(8), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static ClaimsPrincipal ValidateToken(string token)
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Issuer,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
        }
    }
}