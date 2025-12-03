using System.ComponentModel.DataAnnotations;

namespace App_BodyCorp.Models
{
    public class Produto
    {
        public int ProdutoId { get; set; }
        [Required]
        public string? ProdutoNome { get; set; }
    
        public string? Descricao { get; set; }

        [Required]
        public decimal Preco { get; set; }

        [Required]
        public string? Categoria { get; set; }

        //relacionanmento de itenscompra
         public ICollection<ItemCompra>? ItemCompras { get; set; }        
        }
}
