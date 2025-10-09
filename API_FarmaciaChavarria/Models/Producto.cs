using System.ComponentModel.DataAnnotations;

namespace API_FarmaciaChavarria.Models
{
    public class Producto
    {
        [Key]
        public int id_producto { get; set; }  // Clave primaria
        public string nombre { get; set; }
        public int id_categoria { get; set; }  // Clave foránea
        public int id_laboratorio { get; set; }  // Clave foránea
        public decimal precio { get; set; }
        public int stock { get; set; }
        public int stock_minimo { get; set; }
        public string? efectos_secundarios { get; set; }
        public string? como_usar { get; set; }
        public DateOnly fecha_vencimiento { get; set; }

    }

}
