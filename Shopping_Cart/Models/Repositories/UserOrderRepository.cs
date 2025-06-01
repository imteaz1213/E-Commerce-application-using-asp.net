﻿using Microsoft.EntityFrameworkCore;
using Shopping_Cart.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
namespace Shopping_Cart.Models.Repositories
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly Shopping_CartDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        public UserOrderRepository(Shopping_CartDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<IEnumerable<Order>> UserOrders()
        {
            var userId = GetUserId();
            
            if(string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("user is not logged-in");

            var orders = await _db.Orders
                        .Include(x => x.OrderStatus)
                        .Include(x => x.OrderDetail)
                        .ThenInclude(x => x.Book)
                        .ThenInclude(x => x.Genre)
                        .Where(a => a.UserId == userId)
                        .ToListAsync();
            return orders;
        }
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }


    }

   
}
