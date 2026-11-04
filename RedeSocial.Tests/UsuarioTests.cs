using RedeSocial.Core.Entidades;
using RedeSocial.Core.Exceptions;

namespace RedeSocial.Tests;

[TestClass]
public sealed class UsuarioTests
{
    // Caminho Feliz
    [TestMethod]
    public void CriarUsuario_QuandoTodosDadosCorretos_RetornaObjeto()
    {
        // Arrange - Dados iniciais
        const string nomeEsperado = "Gabriel";
        const string emailEsperado = "gabriel@email.com";
        const string senha = "gabriel123";

        // Act - Ação a ser executada
        var usuarioCriado = new Usuario(nomeEsperado, emailEsperado, senha);

        // Assert - Afirmação do que deveria acontecer
        Assert.IsNotNull(usuarioCriado); // Meu usuario não é nulo
        Assert.AreEqual(nomeEsperado,
            usuarioCriado.Nome); // Se o nome que eu atribui ao objeto está definitivamente dentro dele
        Assert.AreEqual(emailEsperado,
            usuarioCriado.Email); // Se o email que eu atribui ao objeto está definitivamente dentro dele
    }

    // Caminho Infeliz
    [TestMethod]
    public void CriarUsuario_QuandoNomeNulo_RetornaException()
    {
        // Arrange - Dados iniciais
        const string nomeEsperado = null!;
        const string emailEsperado = "gabriel@email.com";
        const string senha = "gabriel123";

        // Act&Assert - Ação a ser executada dentro da afirmação
        var exception = Assert.Throws<CoreException>(() => { new Usuario(nomeEsperado!, emailEsperado, senha); });

        // Assert
        Assert.Contains("O nome do usuário não pode ser nulo ou estar vazio!", exception.Message);
    }
}