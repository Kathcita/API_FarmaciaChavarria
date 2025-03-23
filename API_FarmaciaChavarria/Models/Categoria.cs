using System.ComponentModel.DataAnnotations;

namespace API_FarmaciaChavarria.Models
{
    public class Categoria
    {
        [Key]
        public int id_categoria { get; set; }

        public string nombre { get; set; }

    }
}
