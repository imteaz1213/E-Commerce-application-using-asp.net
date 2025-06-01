using Microsoft.EntityFrameworkCore;
using Shopping_Cart.Areas.Identity.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping_Cart.Models.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly Shopping_CartDbContext _db;

        public HomeRepository(Shopping_CartDbContext db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return  _db.Genres.ToList();
        }

        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();

            var books = await (from book in _db.Books join genre in _db.Genres
                               on book.GenreId equals genre.Id
                               where string.IsNullOrWhiteSpace(sTerm) || (book != null && book.BookName.ToLower().StartsWith(sTerm)) select new Book
                               {
                                   Id = book.Id,
                                   Image = book.Image,
                                   AuthorName = book.AuthorName,
                                   BookName = book.BookName,
                                   GenreId = book.GenreId,
                                   Price = book.Price,
                                   GenreName = genre.GenreName,
                               }).ToListAsync();  

            if (genreId > 0)
            {
                books = books.Where(a => a.GenreId == genreId).ToList();
            }

            return books;
        }
    }
}




 

