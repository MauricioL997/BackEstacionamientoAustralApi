using System.Collections.Generic;

namespace Data.Entities
{
    public class Cochera
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Deshabilitada { get; set; }
        public bool Eliminada { get; set; }

        // Colección de estacionamientos que pueden estar asociados a esta cochera
        public ICollection<Estacionamiento> Estacionamientos { get; set; }
    }
}
