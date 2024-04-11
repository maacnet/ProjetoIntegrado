using AssinaturasAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace AssinaturasAPI.Context
{
    public class AssinaturaContexto : DbContext
    {
        public AssinaturaContexto(DbContextOptions<AssinaturaContexto> options) : base(options)
        {
        }

        // Defina as propriedades DbSet para as suas entidades (tabelas) aqui
        public DbSet<Assinatura> Assinaturas { get; set; }
    }
}


