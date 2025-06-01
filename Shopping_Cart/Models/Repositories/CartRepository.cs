using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart.Areas.Identity.Data;
using Microsoft.Extensions.Logging;





namespace Shopping_Cart.Models.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly Shopping_CartDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartRepository> _logger;

        public CartRepository(Shopping_CartDbContext db, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor, ILogger<CartRepository> logger)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<int> AddItem(int bookId, int qty)
        {
            string userId = GetUserId();
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");

                var cart = await GetCart(userId);

                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }

                _db.SaveChanges();

                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var book = _db.Books.Find(bookId);
                    if (book == null)
                    {
                        throw new Exception("Invalid book ID");
                    }
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = book.Price  
                    };
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {

            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;

        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new InvalidOperationException("Invalid userid");
            var shoppingCart = await _db.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Book)
                                  .ThenInclude(a => a.Stock)
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Book)
                                  .ThenInclude(a => a.Genre)
                                  .FirstOrDefaultAsync(a => a.UserId == userId);
            return shoppingCart;
        }

        public async Task<int> RemoveItem(int bookId)
        {
           
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new InvalidOperationException("Invalid cart");
               
                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is null)
                    throw new InvalidOperationException("No items in cart");
                else if (cartItem.Quantity == 1)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity = cartItem.Quantity - 1;
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;

        }
        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
                userId = GetUserId();

            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              where cart.UserId == userId
                              select new{cartDetail.Id}).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheckout()
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                //move data from cartdetail to order and orderdetail than we will remove cartdetail
                //entry-order,orderdetail
                //remove-data ,cartdetail
                var userId = GetUserId();
                if(string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");

                var cart = await GetCart(userId);

                if(cart is null)
                    throw new InvalidOperationException("Invalid cart");

                var cartDetail = _db.CartDetails
                                  .Where(a => a.ShoppingCartId == cart.Id)
                                  .ToList();

                if(cartDetail.Count == 0)
                    throw new InvalidOperationException("No items in cart");

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.Now,
                    OrderStatusId = 1
                };
                _db.Orders.Add(order);
                _db.SaveChanges();

                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);
                }
                _db.SaveChanges();
                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }          
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}







