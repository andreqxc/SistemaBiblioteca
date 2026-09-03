using Biblioteca.Models;

namespace Biblioteca.Services
{
    public interface IAutorService
    {
        List<Autor> ObtenerTodos();
        Autor? ObtenerPorId(int id);
        void Actualizar(Autor autor);
        void Eliminar(int id);
    }
}
