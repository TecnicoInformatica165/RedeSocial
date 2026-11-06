using RedeSocial.Api.Dados;
using RedeSocial.Core.Entidades;

namespace RedeSocial.Api.Servicos;

public class PostagemServicos(DbContexto contexto)
{
    public async Task AdicionaAsync(Postagem postagem)
    {
        await contexto.Postagens.AddAsync(postagem);
        await contexto.SaveChangesAsync();
    }
}