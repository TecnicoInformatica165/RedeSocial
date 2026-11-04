namespace RedeSocial.Core.Exceptions;

public class CoreException(string message) : Exception(message)
{
    // Metodo construtor que recebe uma mensagem e passa adiante para a classe Pai
}