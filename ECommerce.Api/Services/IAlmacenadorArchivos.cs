namespace ECommerce.Api.Services
{
    public interface IAlmacenadorArchivos
    {
        Task BorrarArchivo(string? ruta, string contenedor);
        Task<string> Almacenar(string contenedor, IFormFile archivo);

        async Task<string> RemplazarArchivo(string?ruta, string contenedor,IFormFile archivo)
        {
            await BorrarArchivo(ruta, contenedor);
            return await Almacenar(contenedor, archivo);

        }
    }
}
