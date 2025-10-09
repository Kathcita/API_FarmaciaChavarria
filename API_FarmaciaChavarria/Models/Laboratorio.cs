using System.ComponentModel.DataAnnotations;

namespace API_FarmaciaChavarria.Models
{
    public class Laboratorio
    {
        [Key]
        public int id_laboratorio { get; set; }
        public string nombre { get; set; }

    }
}
