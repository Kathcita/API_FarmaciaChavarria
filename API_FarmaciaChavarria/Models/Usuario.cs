using System.ComponentModel.DataAnnotations;

namespace API_FarmaciaChavarria.Models
{
    public class Usuario
    {
        [Key]
       public int id_usuario { get; set; }
       public string nombre { get; set; }
       public int pin { get; set; }
       public string rol { get; set; }
    }
}
