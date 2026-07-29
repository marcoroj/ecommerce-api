namespace ECommerce.Api.DTOs.Products
{
    public class ProductFiltroDto
    {
        public string? Name {  get; set; }
        public bool? StockMenorACinco {  get; set; }
        public bool? TieneImagen { get; set; }
        public string? NombreCategoria { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }


        
    }
}
