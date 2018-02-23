using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Data;
using MyApp.Models;

namespace RazorPagesContacts.Pages
{
    public class CreateModel : PageModel
    {
        private readonly MyAppContext _db;

        public CreateModel(MyAppContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Todo Todo { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _db.Todos.Add(Todo);
            await _db.SaveChangesAsync();
            return RedirectToPage("/Index");
        }
    }
}