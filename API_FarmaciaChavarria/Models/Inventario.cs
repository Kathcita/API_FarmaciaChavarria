using System;
using System.ComponentModel.DataAnnotations;

namespace API_FarmaciaChavarria.Models
{
    public class Inventario
    {
        [Key]
        public int id_movimiento { get; set; }
        public int id_producto { get; set; }
        public string tipo_movimiento { get; set; }
        public int cantidad { get; set; }
        public DateTime fecha_movimiento { get; set; }
        public int id_usuario { get; set; }
    }
}
