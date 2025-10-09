using API_FarmaciaChavarria.ModelsDto;

namespace API_FarmaciaChavarria.Models.PaginationModels
{
    public class ProductoPagedResult
    {
        public List<ProductoDetailedDTO> Productos { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

}
