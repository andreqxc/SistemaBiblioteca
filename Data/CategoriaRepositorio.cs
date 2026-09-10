using Biblioteca.Models;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Data
{
    public class CategoriaRepositorio
    {
        private readonly string _cadena;

        public CategoriaRepositorio(IConfiguration configuration)
        {
            _cadena = configuration.GetConnectionString("BibliotecaDB")!;
        }

        public List<Categoria> Listar()
        {
            var lista = new List<Categoria>();

            using var conexion = new SqlConnection(_cadena);
            using var comando = new SqlCommand("SELECT Id, Nombre, Descripcion FROM Categorias", conexion);

            conexion.Open();
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                lista.Add(new Categoria
                {
                    Id = lector.GetInt32(0),
                    Nombre = lector.GetString(1),
                    Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                });
            }

            return lista;
        }

        public Categoria? ObtenerPorId(int id)
        {
            using var conexion = new SqlConnection(_cadena);
            using var comando = new SqlCommand(
                "SELECT Id, Nombre, Descripcion FROM Categorias WHERE Id = @Id", conexion);

            comando.Parameters.AddWithValue("@Id", id);

            conexion.Open();
            using var lector = comando.ExecuteReader();

            if (!lector.Read()) return null;

            return new Categoria
            {
                Id = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
            };
        }

        public void Insertar(Categoria categoria)
        {
            using var conexion = new SqlConnection(_cadena);
            using var comando = new SqlCommand(
                "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)", conexion);

            comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            comando.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);

            conexion.Open();
            comando.ExecuteNonQuery();
        }

        public void Actualizar(Categoria categoria)
        {
            using var conexion = new SqlConnection(_cadena);
            using var comando = new SqlCommand(
                "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion WHERE Id = @Id", conexion);

            comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            comando.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);
            comando.Parameters.AddWithValue("@Id", categoria.Id);

            conexion.Open();
            comando.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var conexion = new SqlConnection(_cadena);
            using var comando = new SqlCommand("DELETE FROM Categorias WHERE Id = @Id", conexion);

            comando.Parameters.AddWithValue("@Id", id);

            conexion.Open();
            comando.ExecuteNonQuery();
        }
    }
}
