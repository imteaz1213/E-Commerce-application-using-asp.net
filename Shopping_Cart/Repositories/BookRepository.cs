using Microsoft.EntityFrameworkCore;
using Shopping_Cart.Areas.Identity.Data;
using Shopping_Cart.Models;

namespace Shopping_Cart.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly Shopping_CartDbContext _context;   
        public BookRepository(Shopping_CartDbContext context)
        {
            _context = context; 
        }
        public async Task AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBook(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<Book?> GetBookById(int id) => await _context.Books.FindAsync(id);

        public async Task<IEnumerable<Book>> GetBooks() => await _context.Books.Include(a => a.Genre).ToListAsync();

        public async Task UpdateBook(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}
