using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SacadoCedentesAPI.Models
{
    public class SacadoCedente
    {
        [Key]
        [Column("numero_SacadoCedentes")]
        public int NumeroSacadoCedentes { get; set; }

        [Column("nome_SacadoCedentes")]
        public string? NomeSacadoCedentes { get; set; }

    }
}
