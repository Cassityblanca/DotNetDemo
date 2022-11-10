using Demo.Interfaces;
using Demo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Demo.Areas.Admin.Pages.Users
{
    public class IndexModel : PageModel
    {
        //dependency injection database
        private readonly IUnitOfWork _unitOfWork;
        //dependency injection for aspnet user
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IEnumerable<ApplicationUser> AppUsers { get; set; }
        public Dictionary<string, List<string>> UserRoles { get; set; }
        public string Message { get; set; }

        //params used for message passing but will be defaults to beign with (false/null)
        public async Task OnGet(string message = "")
        {
            Message = message;

            UserRoles = new Dictionary<string, List<string>>();
            AppUsers = _unitOfWork.ApplicationUser.GetAll();
            foreach (var user in AppUsers)
            {
                //go out and get the user roles for this user
                var userRole = await _userManager.GetRolesAsync(user);
                //add these to the user roles dictionary where the key is the user
                UserRoles.Add(user.Id, userRole.ToList());
            }
        }

        //allow declaration of onpost target - asp handler.  Note how
        public IActionResult OnPostLockUnlock(string id)
        {
            //get target user from list
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

            //if lock is null then it means they are unlocked - so lock them out
            if (user.LockoutEnd == null)
            {
                //this is how long they will be locked out
                user.LockoutEnd = DateTime.Now.AddYears(100);
                user.LockoutEnabled = true;
            }
            //assume already locked and you are trying to unlock
            else if (user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now;
                user.LockoutEnabled = false;
            }
            //if anything slips through cracks, catch is to lock
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(100);
            }

            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.Commit();

            //redirect to same page (force refresh)
            return RedirectToPage();
        }
    }
}
