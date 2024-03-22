using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BancosAPI.Models;
using BancosAPI.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace BancoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BancosController : ControllerBase
    {
        private readonly BancoContexto _context;
        private readonly IMemoryCache _cache;

        public BancosController(BancoContexto context,IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/Bancos
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Banco>>> GetBancos()
        {
            try
            {

                //  obter os bancos do cache
                if (!_cache.TryGetValue("bancos", out List<Banco>? bancos))
                {
                    // Se não estiver no cache, buscando do banco de dados
                    bancos = await _context.Bancos.ToListAsync();

                    // Adicionando à memória cache com uma chave "bancos" e expire após 10 minutos
                    _cache.Set("bancos", bancos, TimeSpan.FromMinutes(10));
                }

                return Ok(bancos); // Retorna a lista de bancos com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar bancos", error = ex.Message });
            }
        }


        // GET: api/Bancos/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Banco>> GetBanco(int id)
        {
            try
            {
                var banco = await _context.Bancos.FindAsync(id);
                if (banco == null)
                {
                    return NotFound(new { message = "Banco não encontrado" });
                }
                return Ok(banco); // Retorna o banco com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar banco", error = ex.Message });
            }
        }

        // POST: api/Bancos
        [HttpPost]
        [Authorize (Roles = "InserirBancos")]
        public async Task<ActionResult<Banco>> PostBanco(Banco banco)
        {
            try
            {
                _context.Bancos.Add(banco);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetBanco), new { id = banco.NumeroBancos }, banco); // Retorna o banco criado com status 201 (Created)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar banco", error = ex.Message });
            }
        }

        // PUT: api/Bancos/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBanco(int id, Banco banco)
        {
            if (id != banco.NumeroBancos)
            {
                return BadRequest(new { message = "IDs não correspondem" }); // Retorna mensagem de erro com status 400 (Bad Request)
            }

            try
            {
                _context.Entry(banco).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na atualização
                return Ok(new { message = "Banco alterado", banco }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar banco", error = ex.Message });
            }
        }

        // DELETE: api/Bancos/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBanco(int id)
        {
            try
            {
                var banco = await _context.Bancos.FindAsync(id);
                if (banco == null)
                {
                    return NotFound(new { message = "Banco não encontrado" }); // Retorna mensagem de erro com status 404 (Not Found)
                }

                _context.Bancos.Remove(banco);
                await _context.SaveChangesAsync();
                //return NoContent(); // Retorna status 204 (No Content) para sucesso na exclusão
                return Ok(new { message = "Banco Excluído", banco }); // Retorna status 204 (No Content) para sucesso na atualização
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir banco", error = ex.Message });
            }
        }
    }
}
