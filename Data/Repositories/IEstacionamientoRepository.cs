using Data.context;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositories
{
    public class IEstacionamientoRepository 
    {
        private readonly ApplicationContext _context;
        private readonly ITarifaRepository _tarifaRepository;

        public IEstacionamientoRepository(ApplicationContext context, ITarifaRepository tarifaRepository)
        {
            _context = context;
            _tarifaRepository = tarifaRepository;
        }

        public List<Estacionamiento> GetAllEstacionamientos()
        {
            return _context.Estacionamientos.Where(e => !e.Eliminado).ToList();
        }

        public Estacionamiento GetEstacionamientoById(int id)
        {
            return _context.Estacionamientos.FirstOrDefault(e => e.Id == id && !e.Eliminado);
        }

        // Obtener las últimas transacciones de estacionamiento
        public List<Estacionamiento> GetUltimasTransacciones(int cantidad)
        {
            return _context.Estacionamientos
                .Where(e => e.HoraEgreso != null && !e.Eliminado) // Filtra los estacionamientos con HoraEgreso y no eliminados
                .OrderByDescending(e => e.HoraIngreso) // Ordena de más reciente a más antiguo
                .Take(cantidad) // Toma la cantidad especificada
                .ToList();
        }

        public int AddEstacionamiento(Estacionamiento estacionamiento)
        {
            _context.Estacionamientos.Add(estacionamiento);
            _context.SaveChanges();
            return estacionamiento.Id;
        }

        public void UpdateEstacionamiento(Estacionamiento estacionamiento)
        {
            _context.Estacionamientos.Update(estacionamiento);
            _context.SaveChanges();
        }

        public void DeleteEstacionamiento(int id)
        {
            var estacionamiento = _context.Estacionamientos.Find(id);
            if (estacionamiento != null)
            {
                estacionamiento.Eliminado = true;
                _context.SaveChanges();
            }
        }

        // Nueva funcionalidad: abrir cochera
        public int AbrirEstacionamiento(string patente, int idUsuarioIngreso, int idCochera)
        {
            var cocheraOcupada = _context.Estacionamientos
                .Any(e => e.IdCochera == idCochera && e.HoraEgreso == null && !e.Eliminado);

            if (cocheraOcupada)
            {
                throw new InvalidOperationException("La cochera ya está ocupada.");
            }

            var nuevoEstacionamiento = new Estacionamiento
            {
                Patente = patente,
                IdUsuarioIngreso = idUsuarioIngreso,
                IdCochera = idCochera,
                HoraIngreso = DateTime.Now
            };

            _context.Estacionamientos.Add(nuevoEstacionamiento);
            _context.SaveChanges();
            return nuevoEstacionamiento.Id;
        }

        // Nueva funcionalidad: cerrar cochera
        public void CerrarEstacionamiento(string patente, int idUsuarioEgreso)
        {
            var estacionamiento = _context.Estacionamientos
                .FirstOrDefault(e => e.Patente == patente && e.HoraEgreso == null && !e.Eliminado);

            if (estacionamiento == null)
            {
                throw new InvalidOperationException("No hay un estacionamiento activo para la patente dada.");
            }

            var tiempoEstacionado = DateTime.Now - estacionamiento.HoraIngreso;
            var minutosEstacionados = tiempoEstacionado.TotalMinutes;

            decimal costo = CalcularCosto(minutosEstacionados);

            estacionamiento.HoraEgreso = DateTime.Now;
            estacionamiento.IdUsuarioEgreso = idUsuarioEgreso;
            estacionamiento.Costo = costo;

            _context.Estacionamientos.Update(estacionamiento);
            _context.SaveChanges();
        }

        private decimal CalcularCosto(double minutosEstacionados)
        {
            var tarifaMediaHora = _tarifaRepository.GetAllTarifas()
                .FirstOrDefault(t => t.Descripcion == "MEDIA HORA");
            var tarifaUnaHora = _tarifaRepository.GetAllTarifas()
                .FirstOrDefault(t => t.Descripcion == "UNA HORA");
            var tarifaValorHora = _tarifaRepository.GetAllTarifas()
                .FirstOrDefault(t => t.Descripcion == "VALOR HORA");

            if (tarifaMediaHora == null || tarifaUnaHora == null || tarifaValorHora == null)
            {
                throw new InvalidOperationException("Las tarifas requeridas no están definidas en la base de datos.");
            }

            if (minutosEstacionados <= 30)
            {
                return tarifaMediaHora.Valor;
            }
            else if (minutosEstacionados <= 60)
            {
                return tarifaUnaHora.Valor;
            }
            else
            {
                return (decimal)(minutosEstacionados / 60) * tarifaValorHora.Valor;
            }
        }
    }
}
