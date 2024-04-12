using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RelatoriosAPI.Models
{
    public class Relatorio
    {
        [Key]
        [Column("numero_Relatorios")]
        public int NumeroRelatorios { get; set; }

        [Column("nome_Relatorios")]
        public string? NomeRelatorios { get; set; }

    }
}
