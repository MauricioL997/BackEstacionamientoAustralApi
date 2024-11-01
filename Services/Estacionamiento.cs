using Common.Dtos;
using Data.Entities;
using Data.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class EstacionamientoService
    {
        private readonly IEstacionamientoRepository _repository;

        public EstacionamientoService(IEstacionamientoRepository repository)
        {
            _repository = repository;
        }

        // Obtener todos los estacionamientos que no han sido eliminados
        public List<EstacionamientoDto> GetAllEstacionamientos()
        {
            return _repository.GetAllEstacionamientos()
                              .Where(e => !e.Eliminado) // Filtra los eliminados
                              .Select(estacionamiento => MapToDto(estacionamiento))
                              .ToList();
        }

        // Obtener un estacionamiento por su ID
        public EstacionamientoDto GetEstacionamientoById(int id)
        {
            var estacionamiento = _repository.GetEstacionamientoById(id);
            return estacionamiento != null ? MapToDto(estacionamiento) : null;
        }

        // Obtener las últimas transacciones de estacionamiento con información de cochera
        public List<EstacionamientoDto> GetUltimasTransacciones(int cantidad)
        {
            return _repository.GetAllEstacionamientos()
                              .Where(e => e.HoraEgreso != null && !e.Eliminado) // Filtra los que tienen HoraEgreso y no están eliminados
                              .OrderByDescending(e => e.HoraIngreso) // Ordena por HoraIngreso de más reciente a más antiguo
                              .Take(cantidad) // Toma la cantidad solicitada
                              .Select(estacionamiento => MapToDtoWithCochera(estacionamiento)) // Mapea incluyendo la información de la cochera
                              .ToList();
        }

        // Agregar un nuevo estacionamiento
        public int AddEstacionamiento(EstacionamientoDto estacionamientoDto)
        {
            var estacionamiento = MapToEntity(estacionamientoDto);
            return _repository.AddEstacionamiento(estacionamiento);
        }

        // Actualizar un estacionamiento existente
        public void UpdateEstacionamiento(EstacionamientoDto estacionamientoDto)
        {
            var estacionamiento = MapToEntity(estacionamientoDto);
            _repository.UpdateEstacionamiento(estacionamiento);
        }

        // Eliminar un estacionamiento (soft delete)
        public void DeleteEstacionamiento(int id)
        {
            _repository.DeleteEstacionamiento(id);
        }

        // Abrir una cochera
        public int AbrirEstacionamiento(string patente, int idUsuarioIngreso, int idCochera)
        {
            return _repository.AbrirEstacionamiento(patente, idUsuarioIngreso, idCochera);
        }

        // Cerrar una cochera y calcular el costo
        public void CerrarEstacionamiento(string patente, int idUsuarioEgreso)
        {
            _repository.CerrarEstacionamiento(patente, idUsuarioEgreso);
        }

        // Método de mapeo de entidad a DTO con información de cochera
        private EstacionamientoDto MapToDtoWithCochera(Estacionamiento estacionamiento)
        {
            return new EstacionamientoDto
            {
                Id = estacionamiento.Id,
                Patente = estacionamiento.Patente,
                HoraIngreso = estacionamiento.HoraIngreso,
                HoraEgreso = estacionamiento.HoraEgreso,
                Costo = estacionamiento.Costo,
                IdUsuarioIngreso = estacionamiento.IdUsuarioIngreso,
                IdUsuarioEgreso = estacionamiento.IdUsuarioEgreso,
                IdCochera = estacionamiento.IdCochera,
                Eliminado = estacionamiento.Eliminado,
                Cochera = estacionamiento.Cochera != null ? new CocheraDto
                {
                    Id = estacionamiento.Cochera.Id,
                    Descripcion = estacionamiento.Cochera.Descripcion,
                    Deshabilitada = estacionamiento.Cochera.Deshabilitada,
                    Eliminada = estacionamiento.Cochera.Eliminada
                } : null
            };
        }

        // Método de mapeo de entidad a DTO (sin información de cochera)
        private EstacionamientoDto MapToDto(Estacionamiento estacionamiento)
        {
            return new EstacionamientoDto
            {
                Id = estacionamiento.Id,
                Patente = estacionamiento.Patente,
                HoraIngreso = estacionamiento.HoraIngreso,
                HoraEgreso = estacionamiento.HoraEgreso,
                Costo = estacionamiento.Costo,
                IdUsuarioIngreso = estacionamiento.IdUsuarioIngreso,
                IdUsuarioEgreso = estacionamiento.IdUsuarioEgreso,
                IdCochera = estacionamiento.IdCochera,
                Eliminado = estacionamiento.Eliminado
            };
        }

        // Método de mapeo de DTO a entidad
        private Estacionamiento MapToEntity(EstacionamientoDto estacionamientoDto)
        {
            return new Estacionamiento
            {
                Id = estacionamientoDto.Id,
                Patente = estacionamientoDto.Patente,
                HoraIngreso = estacionamientoDto.HoraIngreso,
                HoraEgreso = estacionamientoDto.HoraEgreso,
                Costo = estacionamientoDto.Costo,
                IdUsuarioIngreso = estacionamientoDto.IdUsuarioIngreso,
                IdUsuarioEgreso = estacionamientoDto.IdUsuarioEgreso,
                IdCochera = estacionamientoDto.IdCochera,
                Eliminado = estacionamientoDto.Eliminado
            };
        }
    }
}
