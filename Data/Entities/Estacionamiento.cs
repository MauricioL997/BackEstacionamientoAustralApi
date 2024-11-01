using System;

namespace Data.Entities
{
    public class Estacionamiento
    {
        public int Id { get; set; }
        public string Patente { get; set; }
        public DateTime HoraIngreso { get; set; }
        public DateTime? HoraEgreso { get; set; }
        public decimal Costo { get; set; }
        public int IdUsuarioIngreso { get; set; }
        public int IdUsuarioEgreso { get; set; }
        public int IdCochera { get; set; } // Clave foránea de Cochera
        public bool Eliminado { get; set; }

        // Propiedad de navegación hacia Cochera
        public Cochera Cochera { get; set; }
    }
}
