using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RedeSocial.Api.Dados;
using RedeSocial.Api.DTOs;
using RedeSocial.Api.Servicos;
using RedeSocial.Core.Entidades;
using RedeSocial.Core.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var stringConexao = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddMySql<DbContexto>(stringConexao, ServerVersion.AutoDetect(stringConexao));

// Cadastro de Servicos Customizados
builder.Services.AddScoped<UsuarioServicos>();
builder.Services.AddScoped<PostagemServicos>();

// Cadastro de JWT
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenServicos.Chave)),
            ValidateIssuer = false,
            ValidIssuer = TokenServicos.Emissor,
            ValidateAudience = false,
            ValidAudience = TokenServicos.Audiencia,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();
else app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/status", () => Results.Ok(new
{
    Status = "online"
}));

// Criação de usuário
app.MapPost("/usuario", async (CadastroUsuarioDto body, UsuarioServicos usuarioServicos) =>
{
    try
    {
        var novoUsuario = new Usuario(body.Nome, body.Email, body.Senha);
        await usuarioServicos.AdicionaAsync(novoUsuario);
        return Results.Created();
    }
    catch (CoreException ex)
    {
        return Results.BadRequest(new { Mensagem = ex.Message });
    }
});

// Rota de Login
app.MapPost("/login", async (LoginUsuarioDto body, UsuarioServicos usuarioServicos) =>
{
    try
    {
        var usuarioEncontrado = await usuarioServicos
            .BuscarPorEmailAsync(body.Email);

        if (usuarioEncontrado.HashSenha != body.Senha)
            return Results.BadRequest("Usuario ou senha estão errados!");

        var token = TokenServicos.GerarToken(usuarioEncontrado);

        return Results.Ok(new { Token = token });
    }
    catch (UsuarioNaoEncontradoException ex)
    {
        return Results.NotFound(new { Mensagem = ex.Message });
    }
    catch
    {
        return Results.Problem("Ocorreu um erro desconhecido, tente novamente mais tarde.", "Erro no servidor!", 500);
    }
});

app.MapPost("/postagem",
    async (CriaPostagemDto body, ClaimsPrincipal usuario, UsuarioServicos usuarioServicos,
        PostagemServicos postagemServicos) =>
    {
        try
        {
            if (int.TryParse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var idUsuario))
                return Results.Unauthorized();

            var usuarioEncontrado = await usuarioServicos.BuscarPorIdAsync(idUsuario);

            var novaPostagem = new Postagem(body.Conteudo, usuarioEncontrado);

            await postagemServicos.AdicionaAsync(novaPostagem);

            return Results.Created();
        }
        catch (CoreException ex)
        {
            return Results.BadRequest(new { Mensagem = ex.Message });
        }
    }).RequireAuthorization();

app.Run();