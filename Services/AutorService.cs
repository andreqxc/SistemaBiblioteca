using Biblioteca.Data;
using Biblioteca.Models;

namespace Biblioteca.Services
{
    public class AutorService : IAutorService
    {
        private const string Archivo = "autores.json";

        private readonly List<Autor> _autores = JsonStore.Cargar(Archivo, new List<Autor>
        {
            new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombiana", FechaNacimiento = new DateTime(1927, 3, 6), Activo = true },
            new Autor { Id = 2, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
            new Autor { Id = 3, Nombre = "Mario", Apellido = "Vargas Llosa", Nacionalidad = "Peruana", FechaNacimiento = new DateTime(1936, 3, 28), Activo = false },
            new Autor { Id = 4, Nombre = "Jorge Luis", Apellido = "Borges", Nacionalidad = "Argentina", FechaNacimiento = new DateTime(1899, 8, 24), Activo = false },
            new Autor { Id = 5, Nombre = "Claudia", Apellido = "Lars", Nacionalidad = "Salvadoreña", FechaNacimiento = new DateTime(1899, 12, 20), Activo = true }
        });

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

            JsonStore.Guardar(Archivo, _autores);
        }

        public void Eliminar(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);
            if (autor != null) _autores.Remove(autor);

            JsonStore.Guardar(Archivo, _autores);
        }
    }
}
