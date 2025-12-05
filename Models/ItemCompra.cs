namespace App_BodyCorp.Models
{
    public class ItemCompra
    {
        public int ItemCompraId { get; set; }

        //relacioinamento da compra
        public int CompraId { get; set; }
        public Compra? Compra { get; set; }

        //do produto
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }

        public int Quantidade { get; set; }

        public decimal PrecoUnitario { get; set; }
        //pra fazer a conta

        public decimal TotalItem { get; set; }
    }
}
