using Microsoft.EntityFrameworkCore;
using RedeSocial.Api.Dados;
using RedeSocial.Core.Entidades;
using RedeSocial.Core.Exceptions;

namespace RedeSocial.Api.Servicos;

public class UsuarioServicos(DbContexto contexto)
{
    // Adiciona usuário no banco de dados
    public async Task AdicionaAsync(Usuario modelo)
    {
        await contexto.Usuarios.AddAsync(modelo);
        await contexto.SaveChangesAsync();
    }

    // Busca o usuário pelo email
    public async Task<Usuario> BuscarPorEmailAsync(string email)
    {
        var usuarioEncontrado = await contexto.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Email == email);

        return usuarioEncontrado ?? throw new UsuarioNaoEncontradoException("O usuario não existe!");
    }

    public async Task<Usuario> BuscarPorIdAsync(int idUsuario)
    {
        var usuarioEncontrado = await contexto.Usuarios.FindAsync(idUsuario);

        return usuarioEncontrado ?? throw new UsuarioNaoEncontradoException("O usuario não existe!");
    }
}