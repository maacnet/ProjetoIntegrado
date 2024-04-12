using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContasFinanceirasAPI.Models;
using ContasFinanceirasAPI.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace ContasFinanceirasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContasFinanceirasController : ControllerBase
    {
        private readonly ContasFinanceiraContexto _context;
        private readonly IMemoryCache _cache;

        public ContasFinanceirasController(ContasFinanceiraContexto context,IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/ContasFinanceiras
        [HttpGet]
        [Authorize(Roles = "ConsultarContasFinanceiras")]
        public async Task<ActionResult<IEnumerable<ContasFinanceira>>> GetContasFinanceiras()
        {
            try
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

                //  obter os ContasFinanceiras do cache
                if (!_cache.TryGetValue("ContasFinanceiras", out List<ContasFinanceira>? ContasFinanceiras))
                {
                    // Se não estiver no cache, buscando do ContasFinanceira de dados
                    ContasFinanceiras = await _context.ContasFinanceiras.ToListAsync();

                    // Adicionando à memória cache com uma chave "ContasFinanceiras" e expire após 10 minutos
                    _cache.Set("ContasFinanceiras", ContasFinanceiras, TimeSpan.FromMinutes(10));
                }

                return Ok(ContasFinanceiras); // Retorna a lista de ContasFinanceiras com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar ContasFinanceiras", error = ex.Message });
            }
        }


        // GET: api/ContasFinanceiras/5
        [HttpGet("{id}")]
        [Authorize(Roles = "ConsultaContasFinanceiras")]
        public async Task<ActionResult<ContasFinanceira>> GetContasFinanceira(int id)
        {
            try
            {
                var ContasFinanceira = await _context.ContasFinanceiras.FindAsync(id);
                if (ContasFinanceira == null)
                {
                    return NotFound(new { message = "ContasFinanceira não encontrado" });
                }
                return Ok(ContasFinanceira); // Retorna o ContasFinanceira com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar ContasFinanceira", error = ex.Message });
            }
        }

        // POST: api/ContasFinanceiras
        [HttpPost]
        [Authorize (Roles = "InserirContasFinanceiras")]
        public async Task<ActionResult<ContasFinanceira>> PostContasFinanceira(ContasFinanceira ContasFinanceira)
        {
            try
            {
                _context.ContasFinanceiras.Add(ContasFinanceira);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetContasFinanceira), new { id = ContasFinanceira.NumeroContasFinanceiras }, ContasFinanceira); // Retorna o ContasFinanceira criado com status 201 (Created)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar ContasFinanceira", error = ex.Message });
            }
        }

        // PUT: api/ContasFinanceiras/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutContasFinanceira(int id, ContasFinanceira ContasFinanceira)
        {
            if (id != ContasFinanceira.NumeroContasFinanceiras)
            {
                return BadRequest(new { message = "IDs não correspondem" }); // Retorna mensagem de erro com status 400 (Bad Request)
            }

            try
            {
                _context.Entry(ContasFinanceira).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na atualização
                return Ok(new { message = "ContasFinanceira alterado", ContasFinanceira }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar ContasFinanceira", error = ex.Message });
            }
        }

        // DELETE: api/ContasFinanceiras/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteContasFinanceira(int id)
        {
            try
            {
                var ContasFinanceira = await _context.ContasFinanceiras.FindAsync(id);
                if (ContasFinanceira == null)
                {
                    return NotFound(new { message = "ContasFinanceira não encontrado" }); // Retorna mensagem de erro com status 404 (Not Found)
                }

                _context.ContasFinanceiras.Remove(ContasFinanceira);
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na exclusão
                return Ok(new { message = "ContasFinanceira Excluído", ContasFinanceira }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir ContasFinanceira", error = ex.Message });
            }
        }
    }
}
