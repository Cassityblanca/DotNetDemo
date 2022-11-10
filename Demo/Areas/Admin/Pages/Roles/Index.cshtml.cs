using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Demo.Areas.Admin.Pages.Roles
{
    public class IndexModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(RoleManager<IdentityRole> roleManager)
        {

            _roleManager = roleManager;
        }


        public string Message { get; set; }
        public List<IdentityRole> AllRoles { get; set; }

        public void OnGet(string message = "")
        {

            Message = message;
            AllRoles = _roleManager.Roles.ToList();
        }
    }
}
