namespace API_FarmaciaChavarria.Models.PaginationModels
{
    public class CategoriaPageResult
    {
        public List<Categoria> Categorias { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
