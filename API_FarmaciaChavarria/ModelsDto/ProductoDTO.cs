namespace API_FarmaciaChavarria.ModelsDto
{
    public class ProductoDTO
    {
        public int id_producto { get; set; }
        public string nombre { get; set; }
        public int id_categoria { get; set; }
        public int id_laboratorio { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }
        public DateOnly fecha_vencimiento { get; set; }
    }

}
