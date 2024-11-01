using System;

namespace Common.Dtos
{
    public class EstacionamientoDto
    {
        public int Id { get; set; }
        public string Patente { get; set; } // La patente del auto
        public DateTime HoraIngreso { get; set; } // Maneja mejor las fechas como DateTime
        public DateTime? HoraEgreso { get; set; } // Maneja mejor las fechas como DateTime
        public decimal Costo { get; set; } // Costo del estacionamiento
        public int IdUsuarioIngreso { get; set; } // Usuario que ingresó el vehículo
        public int IdUsuarioEgreso { get; set; } // Usuario que retiró el vehículo
        public int IdCochera { get; set; } // Id de la cochera ocupada
        public bool Eliminado { get; set; } // Para control de soft delete

        // Nueva propiedad para incluir detalles de la cochera asociada
        public CocheraDto Cochera { get; set; }
    }
}
