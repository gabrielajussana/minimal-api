namespace MinimalApi.Infraestrutura.Db;

using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;


public class DbContexto : DbContext
{
    private readonly IConfiguration? _configuracaoAppSettings;

    public DbContexto() { } 

    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }

    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConexao = _configuracaoAppSettings?.GetConnectionString("MySql")
                ?? "server=localhost;database=seubanco;user=seuusuario;password=suasenha"; 

            optionsBuilder.UseMySql(
                stringConexao,
                ServerVersion.AutoDetect(stringConexao)
            );
        }
    }
}