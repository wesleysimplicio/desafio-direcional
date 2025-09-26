using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DirecionalApi.Application.DTOs;
using DirecionalApi.Application.Services;

namespace DirecionalApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VendasController : ControllerBase
{
    private readonly VendaService _vendaService;

    public VendasController(VendaService vendaService)
    {
        _vendaService = vendaService;
    }

    /// <summary>
    /// Obter todas as vendas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VendaDto>>> GetAll()
    {
        try
        {
            var vendas = await _vendaService.GetAllAsync();
            return Ok(vendas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obter venda por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VendaDto>> GetById(int id)
    {
        try
        {
            var venda = await _vendaService.GetByIdAsync(id);
            if (venda == null)
                return NotFound(new { message = "Venda não encontrada" });

            return Ok(venda);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obter vendas por cliente
    /// </summary>
    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<VendaDto>>> GetByClienteId(int clienteId)
    {
        try
        {
            var vendas = await _vendaService.GetByClienteIdAsync(clienteId);
            return Ok(vendas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obter vendas por status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<VendaDto>>> GetByStatus(string status)
    {
        try
        {
            var vendas = await _vendaService.GetByStatusAsync(status);
            return Ok(vendas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Criar nova venda
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VendaDto>> Create(CreateVendaDto createVendaDto)
    {
        try
        {
            var venda = await _vendaService.CreateAsync(createVendaDto);
            return CreatedAtAction(nameof(GetById), new { id = venda.VendaId }, venda);
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
    /// Atualizar venda
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<VendaDto>> Update(int id, UpdateVendaDto updateVendaDto)
    {
        try
        {
            var venda = await _vendaService.UpdateAsync(id, updateVendaDto);
            return Ok(venda);
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
    /// Excluir venda
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _vendaService.DeleteAsync(id);
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
