namespace API_FarmaciaChavarria.ModelsDto
{
    public class ProductoDetailedDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public int id_categoria { get; set; }
        public int id_laboratorio { get; set; }
        public string CategoriaNombre { get; set; }
        public string LaboratorioNombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public DateOnly FechaVencimiento { get; set; }
    }

}
