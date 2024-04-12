using LancamentosAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace LancamentosAPI.Context
{
    public class LancamentoContexto : DbContext
    {
        public LancamentoContexto(DbContextOptions<LancamentoContexto> options) : base(options)
        {
        }

        // Defina as propriedades DbSet para as suas entidades (tabelas) aqui
        public DbSet<Lancamento> Lancamentos { get; set; }
    }
}


