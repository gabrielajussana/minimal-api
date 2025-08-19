using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Tests.Dominio.Entidades;

[TestClass]
public class AdministradorServicoTest
{
    private DbContexto CriarContextoTeste()
    {
        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // banco único por teste
            .Options;

        return new DbContexto(options);
    }

    [TestMethod]
    public void TestandoSalvarNovoAdministrador()
    {
        // Arrange
        var context = CriarContextoTeste();

        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste123";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(adm);

        // Assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count);
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
        var context = CriarContextoTeste();

        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);

        // Act 
        administradorServico.Incluir(adm);
        var admDoBanco = administradorServico.BuscaPorId(adm.Id);

        // Assert
        Assert.AreEqual(adm.Id, admDoBanco.Id);
    }
}