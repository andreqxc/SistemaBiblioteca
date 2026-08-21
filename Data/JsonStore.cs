using System.Text.Json;

namespace Biblioteca.Data
{
    public static class JsonStore
    {
        private static readonly string _carpeta = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
        private static readonly JsonSerializerOptions _opciones = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public static List<T> Cargar<T>(string archivo, List<T> valoresPorDefecto)
        {
            Directory.CreateDirectory(_carpeta);
            var ruta = Path.Combine(_carpeta, archivo);

            if (!File.Exists(ruta))
            {
                Guardar(archivo, valoresPorDefecto);
                return valoresPorDefecto;
            }

            var json = File.ReadAllText(ruta);
            var datos = JsonSerializer.Deserialize<List<T>>(json, _opciones);
            return datos ?? valoresPorDefecto;
        }

        public static void Guardar<T>(string archivo, List<T> datos)
        {
            Directory.CreateDirectory(_carpeta);
            var ruta = Path.Combine(_carpeta, archivo);
            var json = JsonSerializer.Serialize(datos, _opciones);
            File.WriteAllText(ruta, json);
        }
    }
}
