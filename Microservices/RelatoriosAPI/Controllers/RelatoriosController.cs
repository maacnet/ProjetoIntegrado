using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RelatoriosAPI.Models;
using RelatoriosAPI.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace RelatoriosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelatoriosController : ControllerBase
    {
        private readonly RelatorioContexto _context;
        private readonly IMemoryCache _cache;

        public RelatoriosController(RelatorioContexto context,IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/Relatorios
        [HttpGet]
        [Authorize(Roles = "ConsultarRelatorios")]
        public async Task<ActionResult<IEnumerable<Relatorio>>> GetRelatorios()
        {
            try
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

                //  obter os Relatorios do cache
                if (!_cache.TryGetValue("Relatorios", out List<Relatorio>? Relatorios))
                {
                    // Se não estiver no cache, buscando do Relatorio de dados
                    Relatorios = await _context.Relatorios.ToListAsync();

                    // Adicionando à memória cache com uma chave "Relatorios" e expire após 10 minutos
                    _cache.Set("Relatorios", Relatorios, TimeSpan.FromMinutes(10));
                }

                return Ok(Relatorios); // Retorna a lista de Relatorios com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar Relatorios", error = ex.Message });
            }
        }


        // GET: api/Relatorios/5
        [HttpGet("{id}")]
        [Authorize(Roles = "ConsultaRelatorios")]
        public async Task<ActionResult<Relatorio>> GetRelatorio(int id)
        {
            try
            {
                var Relatorio = await _context.Relatorios.FindAsync(id);
                if (Relatorio == null)
                {
                    return NotFound(new { message = "Relatorio não encontrado" });
                }
                return Ok(Relatorio); // Retorna o Relatorio com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar Relatorio", error = ex.Message });
            }
        }

        // POST: api/Relatorios
        [HttpPost]
        [Authorize (Roles = "InserirRelatorios")]
        public async Task<ActionResult<Relatorio>> PostRelatorio(Relatorio Relatorio)
        {
            try
            {
                _context.Relatorios.Add(Relatorio);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRelatorio), new { id = Relatorio.NumeroRelatorios }, Relatorio); // Retorna o Relatorio criado com status 201 (Created)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar Relatorio", error = ex.Message });
            }
        }

        // PUT: api/Relatorios/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutRelatorio(int id, Relatorio Relatorio)
        {
            if (id != Relatorio.NumeroRelatorios)
            {
                return BadRequest(new { message = "IDs não correspondem" }); // Retorna mensagem de erro com status 400 (Bad Request)
            }

            try
            {
                _context.Entry(Relatorio).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na atualização
                return Ok(new { message = "Relatorio alterado", Relatorio }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar Relatorio", error = ex.Message });
            }
        }

        // DELETE: api/Relatorios/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRelatorio(int id)
        {
            try
            {
                var Relatorio = await _context.Relatorios.FindAsync(id);
                if (Relatorio == null)
                {
                    return NotFound(new { message = "Relatorio não encontrado" }); // Retorna mensagem de erro com status 404 (Not Found)
                }

                _context.Relatorios.Remove(Relatorio);
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na exclusão
                return Ok(new { message = "Relatorio Excluído", Relatorio }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir Relatorio", error = ex.Message });
            }
        }
    }
}
