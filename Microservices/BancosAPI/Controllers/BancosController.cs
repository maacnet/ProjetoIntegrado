using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BancosAPI.Models;
using BancosAPI.Context;
using Microsoft.AspNetCore.Authorization;

namespace SeuProjeto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BancosController : ControllerBase
    {
        private readonly BancoContexto _context;

        public BancosController(BancoContexto context)
        {
            _context = context;
        }

        // GET: api/Bancos
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Banco>>> GetBancos()
        {
            try
            {
                var bancos = await _context.Bancos.ToListAsync();
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
        [Authorize]
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
