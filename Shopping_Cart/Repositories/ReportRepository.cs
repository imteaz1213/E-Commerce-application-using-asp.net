using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart.Areas.Identity.Data;
using Shopping_Cart.Constants;
using Shopping_Cart.Models.DTOs;

namespace Shopping_Cart.Repositories
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class ReportRepository : IReportRepository
    {
        private readonly Shopping_CartDbContext _context;
        public ReportRepository(Shopping_CartDbContext context)
        {
            _context = context; 
        }
        public async Task<IEnumerable<TopNSoldBookModel>> GetTopNSellingBooksByDate(DateTime startDate, DateTime endDate)
        {
            var startDateParam = new SqlParameter("@startDate", startDate);
            var endDateParam = new SqlParameter("@endDate", endDate);
            var topFiveSoldBooks = await _context.Database.SqlQueryRaw<TopNSoldBookModel>("exec Usp_GetTopNSellingBooksByDate @startDate,@endDate", startDateParam, endDateParam).ToListAsync();
            return topFiveSoldBooks;
        }
    }   
}
