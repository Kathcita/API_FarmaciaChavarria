using System.ComponentModel.DataAnnotations;

namespace API_FarmaciaChavarria.Models
{
    public class Proveedor
    {
        [Key]
        public int id_proveedor { get; set; }
        public string nombre { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
    }
}
