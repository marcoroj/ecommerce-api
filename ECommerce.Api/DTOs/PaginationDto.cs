namespace ECommerce.Api.DTOs
{
    public class PaginationDto
    {
        private readonly int _maxSize = 50;
        public int Page { get; set; } = 1;
        private int _size=10;

        public int Size
        {
            get { return _size; }
            set { _size = value > _maxSize ? _maxSize : value; }

        }
    }
}
