using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RedeSocial.Core.Entidades;
using RedeSocial.Core.Exceptions;

namespace RedeSocial.Api.Servicos;

public class TokenServicos
{
    public static readonly string Chave = Environment.GetEnvironmentVariable("JWT_CHAVE") ??
                                          throw new JwtConfiguracaoException(
                                              "A chave JWT não foi configurada corretamente.");

    public static readonly string Emissor = Environment.GetEnvironmentVariable("JWT_EMISSOR") ??
                                            throw new JwtConfiguracaoException(
                                                "O emissor JWT não foi configurado corretamente.");

    public static readonly string Audiencia = Environment.GetEnvironmentVariable("JWT_AUDIENCIA") ??
                                              throw new JwtConfiguracaoException(
                                                  "A audiencia JWT não foi configurada corretamente.");

    public static string GerarToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var chaveBytes = Encoding.ASCII.GetBytes(Chave);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            ]),
            Audience = Audiencia,
            Issuer = Emissor,
            Expires = DateTime.UtcNow.AddHours(2)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}