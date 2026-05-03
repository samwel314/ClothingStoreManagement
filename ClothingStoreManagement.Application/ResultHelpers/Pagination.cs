namespace ClothingStoreManagement.Application.ResultHelpers
{
    public class Pagination<T>
    {
        public IEnumerable<T> Items { get; set; } = null!;
        public int Next { get; }
        public int Previous { get; }
        public int TotalPages { get; }
        public int pageSize { get; }
        public int pageNumber { get; }
        public Pagination(int totalCount, int pageSize, int pageNumber)
        {
            this.pageSize = pageSize < 5 ? 5 : pageSize;
            this.TotalPages = (int)Math.Ceiling(totalCount / (double)this.pageSize);
            this.pageNumber = pageNumber < 1 || pageNumber > this.TotalPages ? 1 : pageNumber;
            this.Previous = this.pageNumber == 1 ? 0 : this.pageNumber - 1;
            this.Next = this.pageNumber >= this.TotalPages ? 0 : this.pageNumber + 1;
        }
    }
}
