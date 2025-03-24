using System.ComponentModel.DataAnnotations;

namespace API_FarmaciaChavarria.Models
{
    public class Factura
    {
        [Key]
        public int id_factura { get; set; }
        public DateTime fecha_venta { get; set; }
        public decimal total { get; set; }
        public int id_usuario { get; set; }
    }
}
