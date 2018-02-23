using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Models;
using MyApp.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesContacts.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MyAppContext _db;

        public IndexModel(MyAppContext db)
        {
            _db = db;
        }

        public IList<Todo> Todos { get; private set; }

        public async Task OnGetAsync()
        {
            Todos = await _db.Todos.AsNoTracking().ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var todo = await _db.Todos.FindAsync(id);

            if (todo != null)
            {
                _db.Todos.Remove(todo);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}