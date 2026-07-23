namespace ECommerce.Api.Services
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _contextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env,IHttpContextAccessor contextAccessor)
        {
            _env = env;
            _contextAccessor = contextAccessor;
        }

        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var extensionArchivo = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extensionArchivo}";
            string folder = Path.Combine(_env.WebRootPath, contenedor);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nombreArchivo);

            using (var ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                var contenido=ms.ToArray();
                await File.WriteAllBytesAsync(ruta, contenido);
            }

            var request = _contextAccessor.HttpContext!.Request;
            var url = $"{request.Scheme}://{request.Host}";
            var urlArchivo = Path.Combine(url, contenedor, nombreArchivo).Replace("\\","/");
            return urlArchivo;

        }
        
        public Task BorrarArchivo(string? ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return Task.CompletedTask;
            }

            var nombreArchivo = Path.GetFileName(ruta);
            var directorioArchivos=Path.Combine(_env.WebRootPath, nombreArchivo);

            if (!File.Exists(directorioArchivos)) 
            { 
                File.Delete(directorioArchivos);
            
            }
            return Task.CompletedTask;
        }
    }
}
