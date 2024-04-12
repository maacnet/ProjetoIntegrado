using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ContasFinanceirasAPI.Models
{
    public class ContasFinanceira
    {
        [Key]
        [Column("numero_ContasFinanceiras")]
        public int NumeroContasFinanceiras { get; set; }

        [Column("nome_ContasFinanceiras")]
        public string? NomeContasFinanceiras { get; set; }

    }
}
