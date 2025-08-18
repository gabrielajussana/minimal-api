using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

# region Builder
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(
    options =>
    {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
       ); 
    }
);

var app = builder.Build();
#endregion

# region Home
app.MapGet("/", () => Results.Json(new Home{})).WithTags("Home");
#endregion

# region Administradores

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao { Mensagens = new List<string>() };
    if (string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("O email é obrigatório.");
    if (string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagens.Add("A senha é obrigatória.");
    if (administradorDTO.Perfil == null)
        validacao.Mensagens.Add("O Perfil é obrigatório.");

    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    administradorServico.Incluir(administrador);

}).WithTags("Administradores");


app.MapGet("/administradores", (IAdministradorServico administradorServico, [FromQuery] int? pagina = 1) =>
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach (var adm in administradores)
    {
        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms);
}).WithTags("Administradores");


app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscaPorId(id);

    if (administrador == null) return Results.NotFound();

    return Results.Ok(administrador);

}).WithTags("Administradores");


#endregion

# region Veiculos

ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao { 
        Mensagens = new List<string>()
};

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O nome é obrigatório.");
    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("A Marca é obrigatória.");
    if (veiculoDTO.Ano < 1950)
        validacao.Mensagens.Add("Veículo muito antigo, o ano deve ser maior que 1950.");

    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano,
    };

    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).WithTags("Veículos");


app.MapGet("/veiculos", (IVeiculoServico veiculoServico, [FromQuery] int? pagina = 1) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
}).WithTags("Veículos");


app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);

}).WithTags("Veículos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
     var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;       
    veiculo.Ano = veiculoDTO.Ano;
    
    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
    
}).WithTags("Veículos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();

    veiculoServico.Apagar(veiculo);
    
    return Results.NoContent();
    
}).WithTags("Veículos");


#endregion

# region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
