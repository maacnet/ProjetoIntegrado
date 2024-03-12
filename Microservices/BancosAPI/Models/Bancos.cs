using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BancosAPI.Models
{
    public class Banco
    {
        [Key]
        [Column("numero_bancos")]
        public int NumeroBancos { get; set; }

        [Column("nome_bancos")]
        public string? NomeBancos { get; set; }

        [Column("agencia_bancos")]
        public string? AgenciaBancos { get; set; }

        [Column("conta_bancos")]
        public string? ContaBancos { get; set; }

        [Column("codseq_contascontabil")]
        public int? CodSeqContasContabil { get; set; }
    }
}
