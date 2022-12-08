using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
//using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    //reférence une interface donc il faut implementer l'interface en dessous
    public class TokenService : ITokenService
    {
        // voir definititon clé symétric sert pour générer le token et le decrypter
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            //passage des paramétre a verifier toujours sous forme de liste
            var claims = new List<Claim> 
            {
               new Claim(JwtRegisteredClaimNames.NameId, user.UserName) 
            };

            // encryption du token
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            // recupération des element pour decrypter
            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            //creation de la méthode pour encryption de retour
            var tokenHandler = new JwtSecurityTokenHandler();
            // creation du jeton
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // renvoi du jeton avec ses params
            return tokenHandler.WriteToken(token);
        }
    }
}