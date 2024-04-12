using SacadoCedentesAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace SacadoCedentesAPI.Context
{
    public class SacadoCedenteContexto : DbContext
    {
        public SacadoCedenteContexto(DbContextOptions<SacadoCedenteContexto> options) : base(options)
        {
        }

        // Defina as propriedades DbSet para as suas entidades (tabelas) aqui
        public DbSet<SacadoCedente> SacadoCedentes { get; set; }
    }
}


