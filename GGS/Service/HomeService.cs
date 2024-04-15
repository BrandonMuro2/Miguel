using GGS.Engine;
using Microsoft.EntityFrameworkCore;

namespace GGS.Service
{
    public class HomeService
    {
        private readonly NewContext _context;

        public HomeService(NewContext context)
        {
            _context = context;
        }

        public IEnumerable<List> GetList(int UsId)
        {
            UsId = 1; // Esto establece el UsId a 1, lo que no es correcto si quieres usar el parámetro que se pasa a la función.

            var listsWithGames = _context.Lists
                .Where(lst => lst.UsId == UsId)
                .Include(lst => lst.Us) 
                .Include(lst => lst.Games);

            return listsWithGames;
        }

        public IEnumerable<Game> GetGames()
        {

            var games = _context.Games.Include(lst => lst.RequirementPcs).Include(lst => lst.RequirementConsoles)
;

            return games;
        }



    }
}
