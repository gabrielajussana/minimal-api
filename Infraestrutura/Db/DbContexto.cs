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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConexao = _configuracaoAppSettings?.GetConnectionString("mysql")
                ?? "server=localhost;database=seubanco;user=seuusuario;password=suasenha"; // ajuste conforme necessário

            optionsBuilder.UseMySql(
                stringConexao,
                ServerVersion.AutoDetect(stringConexao)
            );
        }
    }
}