using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_FarmaciaChavarria.Models
{
    public class DetalleFactura
    {
        [Key]
        public int id_detalle { get; set; }
        public int id_factura { get; set; }
        public int id_producto { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
        [NotMapped]
        public decimal subtotal { get
            {
                return precio_unitario * cantidad;
            }}
    }
}
