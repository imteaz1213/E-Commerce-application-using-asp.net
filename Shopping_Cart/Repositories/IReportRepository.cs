using Shopping_Cart.Models.DTOs;

namespace Shopping_Cart.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<TopNSoldBookModel>> GetTopNSellingBooksByDate(DateTime startDate, DateTime endDate);
    }
}
