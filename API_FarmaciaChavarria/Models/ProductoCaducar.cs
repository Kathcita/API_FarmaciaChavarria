using System.ComponentModel.DataAnnotations;

namespace API_FarmaciaChavarria.Models
{
    public class ProductoCaducar
    {
        [Key]
        public int id_producto { get; set; }  // Clave primaria y foránea a la vez
        public string nombre { get; set; }
        public DateOnly fecha_vencimiento { get; set; }


    }

}
