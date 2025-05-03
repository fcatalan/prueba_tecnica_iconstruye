using AutoMapper;
using FacturaSii.src.FacturaComponent.Application.UseCases;
using FacturaSii.src.FacturaComponent.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FacturaSii.src.FacturaComponent.Shared.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FacturaSii.src.FacturaComponent.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private readonly EmitirFacturaUseCase _emitirUseCase;
        private readonly IConsultarEstadoUseCase _consultarEstadoUseCase;

        private readonly IMapper _mapper;


        public FacturasController(EmitirFacturaUseCase emitirUseCase, IConsultarEstadoUseCase consultarEstadoUseCase,  IMapper mapper)
        {
            _mapper = mapper;
            _emitirUseCase = emitirUseCase;
            _consultarEstadoUseCase = consultarEstadoUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Emitir([FromBody] FacturaDto dto)
        {
            try{
                var factura = _mapper.Map<Factura>(dto);
                var estado = await _emitirUseCase.EjecutarAsync(factura);
                return Ok(new { dto.Folio, Estado = estado });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("estado/{folio}")]
        public async Task<IActionResult> Estado(int folio)
        {
            var estado = await _consultarEstadoUseCase.ConsultarEstadoAsync(folio);

            if (estado == null)
            {
                return NotFound($"No se encontró una factura con el folio {folio}.");
            }

            return Ok(new { Folio = folio, Estado = estado });
        }
    }
}
