using BancosAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace BancosAPI.Context
{
    public class BancoContexto : DbContext
    {
        public BancoContexto(DbContextOptions<BancoContexto> options) : base(options)
        {
        }

        // Defina as propriedades DbSet para as suas entidades (tabelas) aqui
        public DbSet<Banco> Bancos { get; set; }
    }
}


