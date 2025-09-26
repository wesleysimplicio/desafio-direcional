using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DirecionalApi.Application.DTOs;
using DirecionalApi.Application.Services;

namespace DirecionalApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _clienteService;

    public ClientesController(ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>
    /// Obter todos os clientes
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
    {
        try
        {
            var clientes = await _clienteService.GetAllAsync();
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obter cliente por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        try
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado" });

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Criar novo cliente
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Create(CreateClienteDto createClienteDto)
    {
        try
        {
            var cliente = await _clienteService.CreateAsync(createClienteDto);
            return CreatedAtAction(nameof(GetById), new { id = cliente.ClienteId }, cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar cliente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ClienteDto>> Update(int id, UpdateClienteDto updateClienteDto)
    {
        try
        {
            var cliente = await _clienteService.UpdateAsync(id, updateClienteDto);
            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Excluir cliente
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _clienteService.DeleteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }
}
