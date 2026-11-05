using Microsoft.EntityFrameworkCore;
using RedeSocial.Core.Entidades;

namespace RedeSocial.Api.Dados;

public class DbContexto(DbContextOptions<DbContexto> opcoesContexto) : DbContext(opcoesContexto)
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Postagem> Postagens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().Property(usuario => usuario.Email).HasColumnType("VARCHAR").HasMaxLength(255)
            .IsRequired();
        modelBuilder.Entity<Usuario>().Property(usuario => usuario.Nome).HasColumnType("VARCHAR").HasMaxLength(255)
            .IsRequired();

        modelBuilder.Entity<Postagem>().Property(postagem => postagem.Conteudo).HasColumnType("VARCHAR")
            .HasMaxLength(2000);
        modelBuilder.Entity<Postagem>().HasOne(postagem => postagem.Usuario).WithMany(usuario => usuario.Postagens).
            HasForeignKey(postagem => postagem.UsuarioId).HasPrincipalKey(usuario => usuario.Id);

    }
}