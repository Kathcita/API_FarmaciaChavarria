namespace API_FarmaciaChavarria.Models.PaginationModels
{
    public class ProveedorPagedResult
    {
        public List<Proveedor> Proveedores { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
