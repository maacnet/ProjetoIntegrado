using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SacadoCedentesAPI.Models;
using SacadoCedentesAPI.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace SacadoCedentesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SacadoCedentesController : ControllerBase
    {
        private readonly SacadoCedenteContexto _context;
        private readonly IMemoryCache _cache;

        public SacadoCedentesController(SacadoCedenteContexto context,IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/SacadoCedentes
        [HttpGet]
        [Authorize(Roles = "ConsultarSacadoCedentes")]
        public async Task<ActionResult<IEnumerable<SacadoCedente>>> GetSacadoCedentes()
        {
            try
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

                //  obter os SacadoCedentes do cache
                if (!_cache.TryGetValue("SacadoCedentes", out List<SacadoCedente>? SacadoCedentes))
                {
                    // Se não estiver no cache, buscando do SacadoCedente de dados
                    SacadoCedentes = await _context.SacadoCedentes.ToListAsync();

                    // Adicionando à memória cache com uma chave "SacadoCedentes" e expire após 10 minutos
                    _cache.Set("SacadoCedentes", SacadoCedentes, TimeSpan.FromMinutes(10));
                }

                return Ok(SacadoCedentes); // Retorna a lista de SacadoCedentes com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar SacadoCedentes", error = ex.Message });
            }
        }


        // GET: api/SacadoCedentes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "ConsultaSacadoCedentes")]
        public async Task<ActionResult<SacadoCedente>> GetSacadoCedente(int id)
        {
            try
            {
                var SacadoCedente = await _context.SacadoCedentes.FindAsync(id);
                if (SacadoCedente == null)
                {
                    return NotFound(new { message = "SacadoCedente não encontrado" });
                }
                return Ok(SacadoCedente); // Retorna o SacadoCedente com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar SacadoCedente", error = ex.Message });
            }
        }

        // POST: api/SacadoCedentes
        [HttpPost]
        [Authorize (Roles = "InserirSacadoCedentes")]
        public async Task<ActionResult<SacadoCedente>> PostSacadoCedente(SacadoCedente SacadoCedente)
        {
            try
            {
                _context.SacadoCedentes.Add(SacadoCedente);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetSacadoCedente), new { id = SacadoCedente.NumeroSacadoCedentes }, SacadoCedente); // Retorna o SacadoCedente criado com status 201 (Created)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar SacadoCedente", error = ex.Message });
            }
        }

        // PUT: api/SacadoCedentes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutSacadoCedente(int id, SacadoCedente SacadoCedente)
        {
            if (id != SacadoCedente.NumeroSacadoCedentes)
            {
                return BadRequest(new { message = "IDs não correspondem" }); // Retorna mensagem de erro com status 400 (Bad Request)
            }

            try
            {
                _context.Entry(SacadoCedente).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na atualização
                return Ok(new { message = "SacadoCedente alterado", SacadoCedente }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar SacadoCedente", error = ex.Message });
            }
        }

        // DELETE: api/SacadoCedentes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSacadoCedente(int id)
        {
            try
            {
                var SacadoCedente = await _context.SacadoCedentes.FindAsync(id);
                if (SacadoCedente == null)
                {
                    return NotFound(new { message = "SacadoCedente não encontrado" }); // Retorna mensagem de erro com status 404 (Not Found)
                }

                _context.SacadoCedentes.Remove(SacadoCedente);
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na exclusão
                return Ok(new { message = "SacadoCedente Excluído", SacadoCedente }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir SacadoCedente", error = ex.Message });
            }
        }
    }
}
