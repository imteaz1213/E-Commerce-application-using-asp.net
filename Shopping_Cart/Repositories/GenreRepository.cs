using Microsoft.EntityFrameworkCore;
using Shopping_Cart.Areas.Identity.Data;
using Shopping_Cart.Models;

namespace Shopping_Cart.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly Shopping_CartDbContext _context;
        public GenreRepository(Shopping_CartDbContext context)
        {
            _context = context;
        }
        public async Task AddGenre(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteGenre(Genre genre)
        {
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }
        public async Task<Genre?> GetGenreById(int id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task UpdateGenre(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }
    }   
}
