﻿using Shopping_Cart.Models.DTOs;

namespace Shopping_Cart.Models.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int bookId,int qty);
        Task<int> RemoveItem(int bookId);

        Task<ShoppingCart> GetUserCart();

        Task<ShoppingCart> GetCart(string userId);

        Task<int> GetCartItemCount(string userId = "");



        Task<bool> DoCheckout(CheckoutModel model);
    }
}
