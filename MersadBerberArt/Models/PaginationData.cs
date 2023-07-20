namespace MersadBerberArt.Models
{
    public class PaginationData
    {
        public PaginationData()
        {
            PageIndex = 1; 
            PageSize = 10;
        }

        public PaginationData(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
    }
}
