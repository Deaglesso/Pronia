namespace Pronia.Areas.Admin.ViewModel
{
    public class PaginationVM<T>
        where T : class,new()
    {
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int Limit { get; set; }
        public List<T> Items { get; set; }
    }
}
