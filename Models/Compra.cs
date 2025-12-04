using System.Data;

namespace App_BodyCorp.Models
{
    public class Compra
    {
        public int CompraId { get; set; }

        public DateTime DataCompra { get; set; } //pra dizer quando ocorreu a compra

        //relaacionamento do cliente
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        //relacionamento com o itens compra, uma compra só pode ter varios itens
        public ICollection<ItemCompra>? Itens { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
