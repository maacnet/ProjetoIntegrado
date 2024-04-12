using RelatoriosAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace RelatoriosAPI.Context
{
    public class RelatorioContexto : DbContext
    {
        public RelatorioContexto(DbContextOptions<RelatorioContexto> options) : base(options)
        {
        }

        // Defina as propriedades DbSet para as suas entidades (tabelas) aqui
        public DbSet<Relatorio> Relatorios { get; set; }
    }
}


