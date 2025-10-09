namespace API_FarmaciaChavarria.ModelsDto
{
    public class DetalleCompraDTO
    {
        public int id_compra { get; set; }
        public int id_producto { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
    }
}
