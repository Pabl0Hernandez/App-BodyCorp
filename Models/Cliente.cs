using Microsoft.Build.Framework;

namespace App_BodyCorp.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        [Required]
        public string? ClienteNome { get; set; }
        [Required]
        public string? CNPJ { get; set; }

        [Required]
        public string? Telefone { get; set; }
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Endereco { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now; //quando cadastrou a empresa

    }
}
