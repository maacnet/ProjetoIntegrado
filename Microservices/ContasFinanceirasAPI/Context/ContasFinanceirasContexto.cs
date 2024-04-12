using ContasFinanceirasAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace ContasFinanceirasAPI.Context
{
    public class ContasFinanceiraContexto : DbContext
    {
        public ContasFinanceiraContexto(DbContextOptions<ContasFinanceiraContexto> options) : base(options)
        {
        }

        // Defina as propriedades DbSet para as suas entidades (tabelas) aqui
        public DbSet<ContasFinanceira> ContasFinanceiras { get; set; }
    }
}


