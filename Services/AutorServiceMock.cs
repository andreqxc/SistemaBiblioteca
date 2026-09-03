using Biblioteca.Models;

namespace Biblioteca.Services
{
    // Segunda implementación de IAutorService (Actividad 5 - Reto).
    // Demuestra que se puede cambiar la fuente de datos sin tocar el controlador,
    // solo cambiando el registro en Program.cs.
    public class AutorServiceMock : IAutorService
    {
        private readonly List<Autor> _autores = new()
        {
            new Autor { Id = 1, Nombre = "Claribel", Apellido = "Alegría", Nacionalidad = "Salvadoreña", FechaNacimiento = new DateTime(1924, 5, 12), Activo = true },
            new Autor { Id = 2, Nombre = "Roque", Apellido = "Dalton", Nacionalidad = "Salvadoreña", FechaNacimiento = new DateTime(1935, 5, 14), Activo = true }
        };

        public List<Autor> ObtenerTodos() => _autores;

        public Autor? ObtenerPorId(int id) => _autores.FirstOrDefault(a => a.Id == id);

        public void Actualizar(Autor autor)
        {
            var existente = _autores.FirstOrDefault(a => a.Id == autor.Id);
            if (existente == null) return;

            existente.Nombre = autor.Nombre;
            existente.Apellido = autor.Apellido;
            existente.Nacionalidad = autor.Nacionalidad;
            existente.FechaNacimiento = autor.FechaNacimiento;
            existente.Activo = autor.Activo;
        }

        public void Eliminar(int id)
        {
            _autores.RemoveAll(a => a.Id == id);
        }
    }
}
