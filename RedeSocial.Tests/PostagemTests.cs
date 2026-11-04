using RedeSocial.Core.Entidades;

namespace RedeSocial.Tests;

[TestClass]
public class PostagemTests
{
    [TestMethod]
    public void CriarPostagem_QuandoDadosCorretos_RetornaObjeto()
    {
        // Arrange
        var usuario = new Usuario("Gabriel", "gabriel@email.com", "gabriel123");

        // Act 
        var postagemCriada = new Postagem("asgsagasg", usuario);
    }
}