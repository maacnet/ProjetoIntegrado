using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AssinaturasAPI.Models
{
    public class Assinatura
    {
        [Key]
        [Column("numero_Assinaturas")]
        public int NumeroAssinaturas { get; set; }

        [Column("nome_Assinaturas")]
        public string? NomeAssinaturas { get; set; }

    }
}
