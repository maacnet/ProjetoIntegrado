using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LancamentosAPI.Models;
using LancamentosAPI.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace LancamentosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LancamentosController : ControllerBase
    {
        private readonly LancamentoContexto _context;
        private readonly IMemoryCache _cache;

        public LancamentosController(LancamentoContexto context,IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/Lancamentos
        [HttpGet]
        [Authorize(Roles = "ConsultarLancamentos")]
        public async Task<ActionResult<IEnumerable<Lancamento>>> GetLancamentos()
        {
            try
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

                //  obter os Lancamentos do cache
                if (!_cache.TryGetValue("Lancamentos", out List<Lancamento>? Lancamentos))
                {
                    // Se não estiver no cache, buscando do Lancamento de dados
                    Lancamentos = await _context.Lancamentos.ToListAsync();

                    // Adicionando à memória cache com uma chave "Lancamentos" e expire após 10 minutos
                    _cache.Set("Lancamentos", Lancamentos, TimeSpan.FromMinutes(10));
                }

                return Ok(Lancamentos); // Retorna a lista de Lancamentos com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar Lancamentos", error = ex.Message });
            }
        }


        // GET: api/Lancamentos/5
        [HttpGet("{id}")]
        [Authorize(Roles = "ConsultaLancamentos")]
        public async Task<ActionResult<Lancamento>> GetLancamento(int id)
        {
            try
            {
                var Lancamento = await _context.Lancamentos.FindAsync(id);
                if (Lancamento == null)
                {
                    return NotFound(new { message = "Lancamento não encontrado" });
                }
                return Ok(Lancamento); // Retorna o Lancamento com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar Lancamento", error = ex.Message });
            }
        }

        // POST: api/Lancamentos
        [HttpPost]
        [Authorize (Roles = "InserirLancamentos")]
        public async Task<ActionResult<Lancamento>> PostLancamento(Lancamento Lancamento)
        {
            try
            {
                _context.Lancamentos.Add(Lancamento);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetLancamento), new { id = Lancamento.NumeroLancamentos }, Lancamento); // Retorna o Lancamento criado com status 201 (Created)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar Lancamento", error = ex.Message });
            }
        }

        // PUT: api/Lancamentos/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutLancamento(int id, Lancamento Lancamento)
        {
            if (id != Lancamento.NumeroLancamentos)
            {
                return BadRequest(new { message = "IDs não correspondem" }); // Retorna mensagem de erro com status 400 (Bad Request)
            }

            try
            {
                _context.Entry(Lancamento).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na atualização
                return Ok(new { message = "Lancamento alterado", Lancamento }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar Lancamento", error = ex.Message });
            }
        }

        // DELETE: api/Lancamentos/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteLancamento(int id)
        {
            try
            {
                var Lancamento = await _context.Lancamentos.FindAsync(id);
                if (Lancamento == null)
                {
                    return NotFound(new { message = "Lancamento não encontrado" }); // Retorna mensagem de erro com status 404 (Not Found)
                }

                _context.Lancamentos.Remove(Lancamento);
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na exclusão
                return Ok(new { message = "Lancamento Excluído", Lancamento }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir Lancamento", error = ex.Message });
            }
        }
    }
}
