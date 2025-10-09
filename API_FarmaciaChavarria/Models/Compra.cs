using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace API_FarmaciaChavarria.Models
{
    public class Compra
    {
        [Key]
        public int id_compra { get; set; }
        public int id_proveedor { get; set; }
        public DateTime fecha_compra { get; set; }
        public decimal total { get; set; } 
    }
}
