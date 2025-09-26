using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DirecionalApi.Application.DTOs;
using DirecionalApi.Application.Services;

namespace DirecionalApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApartamentosController : ControllerBase
{
    private readonly ApartamentoService _apartamentoService;

    public ApartamentosController(ApartamentoService apartamentoService)
    {
        _apartamentoService = apartamentoService;
    }

    /// <summary>
    /// Obter todos os apartamentos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApartamentoDto>>> GetAll()
    {
        try
        {
            var apartamentos = await _apartamentoService.GetAllAsync();
            return Ok(apartamentos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obter apartamento por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApartamentoDto>> GetById(int id)
    {
        try
        {
            var apartamento = await _apartamentoService.GetByIdAsync(id);
            if (apartamento == null)
                return NotFound(new { message = "Apartamento não encontrado" });

            return Ok(apartamento);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obter apartamentos por status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<ApartamentoDto>>> GetByStatus(string status)
    {
        try
        {
            var apartamentos = await _apartamentoService.GetByStatusAsync(status);
            return Ok(apartamentos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Criar novo apartamento
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApartamentoDto>> Create(CreateApartamentoDto createApartamentoDto)
    {
        try
        {
            var apartamento = await _apartamentoService.CreateAsync(createApartamentoDto);
            return CreatedAtAction(nameof(GetById), new { id = apartamento.ApartamentoId }, apartamento);
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
    /// Atualizar apartamento
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApartamentoDto>> Update(int id, UpdateApartamentoDto updateApartamentoDto)
    {
        try
        {
            var apartamento = await _apartamentoService.UpdateAsync(id, updateApartamentoDto);
            return Ok(apartamento);
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
    /// Excluir apartamento
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _apartamentoService.DeleteAsync(id);
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
