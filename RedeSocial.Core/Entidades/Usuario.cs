using System.Text.RegularExpressions;
using RedeSocial.Core.Exceptions;

namespace RedeSocial.Core.Entidades;

public class Usuario
{
    // Propriedades constantes - Usadas para que os códigos fora deste arquivo conheçam o limite de informações importantes
    public const int MINIMO_TAMANHO_NOME = 3;

    public const int MINIMO_TAMANHO_SENHA = 8;

    // Construtor protegido - Para o EFCore
    protected Usuario() // Necessário e utilizado pelo EFCore
    {
    }

    // Construtor publico - Para os demais códigos
    public Usuario(string nome, string email, string senha)
    {
        // Valida os dados dentro de cada metodo respectivo. Os metodos tem a responsabilidade de avaliar se a informação é válida ou não.
        ValidarNome(nome);
        ValidarEmail(email);
        ValidarSenha(senha);
    }

    // Inicio da criação de PROPRIEDADES da classe
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }

    public string HashSenha { get; private set; }
    // Fim da criação de PROPRIEDADES da classe

    private void ValidarSenha(string senha)
    {
        // Avalia se a string é nula ou só contém espaços vazios
        if (string.IsNullOrWhiteSpace(senha))
            throw new CoreException("A senha do usuário não pode ser nula ou estar vazia!");

        // Remove espaços adicionas que o usuário possa ter colocado
        senha = senha.Trim();

        // Testa, através da propriedade Length da string, se a quantidade de letras (neste caso dentro de senha) é menor que o mínimo aceitável
        if (senha.Length < MINIMO_TAMANHO_SENHA)
            throw new CoreException(
                $"A senha do usuário precisa conter no mínimo {MINIMO_TAMANHO_SENHA} caracteres!");

        HashSenha = senha;
    }

    private void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new CoreException("O email do usuário não pode ser nulo ou estar vazio!");

        email = email.Trim();

        // Regex procura por um padrão dentro de uma string e se achar, retorna TRUE devido o IsMatch. Neste caso, se o usuário enviar um e-mail válido, ele vai retornar TRUE, mas o ! antes da palavra Regex faz o valor verdadeiro se torna falso. Ou seja, ele irá pegar os emails que não são válidos.
        if (!Regex.IsMatch(email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase))
            throw new CoreException("O email do usuário está inválido!");

        Email = email;
    }

    private void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new CoreException("O nome do usuário não pode ser nulo ou estar vazio!");

        nome = nome.Trim();

        if (Regex.IsMatch(nome, "/[^A-Za-zÀ-ÿ\\s]/gm", RegexOptions.IgnoreCase))
            throw new CoreException("O nome do usuário possui caracteres inválidos!");

        if (nome.Length < MINIMO_TAMANHO_NOME)
            throw new CoreException($"O nome do usuário deve conter no mínimo {MINIMO_TAMANHO_NOME} caracteres!");

        Nome = nome;
    }
}