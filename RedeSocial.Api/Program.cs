using Microsoft.EntityFrameworkCore;
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

// Cadastro de JWT

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

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

app.Run();