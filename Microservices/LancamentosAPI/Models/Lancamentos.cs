using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LancamentosAPI.Models
{
    public class Lancamento
    {
        [Key]
        [Column("numero_Lancamentos")]
        public int NumeroLancamentos { get; set; }

        [Column("nome_Lancamentos")]
        public string? NomeLancamentos { get; set; }

    }
}
