using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssinaturasAPI.Models;
using AssinaturasAPI.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace AssinaturasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssinaturasController : ControllerBase
    {
        private readonly AssinaturaContexto _context;
        private readonly IMemoryCache _cache;

        public AssinaturasController(AssinaturaContexto context,IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/Assinaturas
        [HttpGet]
        [Authorize(Roles = "ConsultarAssinaturas")]
        public async Task<ActionResult<IEnumerable<Assinatura>>> GetAssinaturas()
        {
            try
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

                //  obter os Assinaturas do cache
                if (!_cache.TryGetValue("Assinaturas", out List<Assinatura>? Assinaturas))
                {
                    // Se não estiver no cache, buscando do assinatura de dados
                    Assinaturas = await _context.Assinaturas.ToListAsync();

                    // Adicionando à memória cache com uma chave "Assinaturas" e expire após 10 minutos
                    _cache.Set("Assinaturas", Assinaturas, TimeSpan.FromMinutes(10));
                }

                return Ok(Assinaturas); // Retorna a lista de Assinaturas com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar Assinaturas", error = ex.Message });
            }
        }


        // GET: api/Assinaturas/5
        [HttpGet("{id}")]
        [Authorize(Roles = "ConsultaAssinaturas")]
        public async Task<ActionResult<Assinatura>> Getassinatura(int id)
        {
            try
            {
                var assinatura = await _context.Assinaturas.FindAsync(id);
                if (assinatura == null)
                {
                    return NotFound(new { message = "assinatura não encontrado" });
                }
                return Ok(assinatura); // Retorna o assinatura com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar assinatura", error = ex.Message });
            }
        }

        // POST: api/Assinaturas
        [HttpPost]
        [Authorize (Roles = "InserirAssinaturas")]
        public async Task<ActionResult<Assinatura>> Postassinatura(Assinatura Assinatura)
        {
            try
            {
                _context.Assinaturas.Add(Assinatura);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Getassinatura), new { id = Assinatura.NumeroAssinaturas }, Assinatura); // Retorna o assinatura criado com status 201 (Created)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar assinatura", error = ex.Message });
            }
        }

        // PUT: api/Assinaturas/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Putassinatura(int id, Assinatura assinatura)
        {
            if (id != assinatura.NumeroAssinaturas)
            {
                return BadRequest(new { message = "IDs não correspondem" }); // Retorna mensagem de erro com status 400 (Bad Request)
            }

            try
            {
                _context.Entry(assinatura).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na atualização
                return Ok(new { message = "assinatura alterado", assinatura }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar assinatura", error = ex.Message });
            }
        }

        // DELETE: api/Assinaturas/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Deleteassinatura(int id)
        {
            try
            {
                var assinatura = await _context.Assinaturas.FindAsync(id);
                if (assinatura == null)
                {
                    return NotFound(new { message = "assinatura não encontrado" }); // Retorna mensagem de erro com status 404 (Not Found)
                }

                _context.Assinaturas.Remove(assinatura);
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na exclusão
                return Ok(new { message = "assinatura Excluído", assinatura }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir assinatura", error = ex.Message });
            }
        }
    }
}
