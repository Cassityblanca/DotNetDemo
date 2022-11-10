using Demo.Interfaces;
using Demo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Demo.Areas.Admin.Pages.Users
{
    public class UpdateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UpdateModel(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public ApplicationUser AppUser { get; set; }
        public List<string> UsersRoles { get; set; }
        public List<string> AllRoles { get; set; }
        public IList<string> OldRoles { get; set; } //for manual tracking

        //get id from update page for current ApplicationUser
        public async Task OnGet(string id)
        {
            //get user
            AppUser = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

            var roles = await _userManager.GetRolesAsync(AppUser);
            UsersRoles = roles.ToList();
            OldRoles = roles.ToList();
            AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
        }

        //Normal post - also make sure you track changes
        public async Task<IActionResult> OnPostAsync()
        {
            //Get list of new roles selected, if any - looking at checked checkboxes
            var newRoles = Request.Form["roles"];

            //update the users roles
            UsersRoles = newRoles.ToList();

            //get current ones in db
            OldRoles = await _userManager.GetRolesAsync(AppUser);

            var rolesToAdd = new List<string>();

            ///Beginning of database handling

            //get current user
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == AppUser.Id);

            //update user information first
            user.FirstName = AppUser.FirstName;
            user.LastName = AppUser.LastName;
            user.Email = AppUser.Email;
            user.PhoneNumber = AppUser.PhoneNumber;
            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.Commit();

            //Update user roles in database
            foreach (var r in UsersRoles)
            {
                //if old role didn't already have
                if (!OldRoles.Contains(r))
                {
                    //add new role
                    rolesToAdd.Add(r);
                }
            }

            //now see if old roles removed
            foreach (var r in OldRoles)
            {
                if (!UsersRoles.Contains(r))
                {
                    //remove from table if was removed
                    var result = await _userManager.RemoveFromRoleAsync(user, r);
                }
            }

            var result1 = await _userManager.AddToRolesAsync(user, rolesToAdd.AsEnumerable());
            return RedirectToPage("./Index", new { message = "Update Successful" });
        }
    }
}
