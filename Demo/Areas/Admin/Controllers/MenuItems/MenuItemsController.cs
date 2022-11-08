using Demo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Demo.Models;
using Demo.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

[Area("Admin")]
public class MenuItemsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    //IF an action that returns a view does NOT excplicitly say which view to call with a string, ex "Index"
    //  Then it will DEFAULT to opening a view tht has the SAME NAME as the action (aka, the same name as the method)

    //IWebHostEnvironment is related to the upsert stuff
    public MenuItemsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)//Dependency Injection & path to wwwroot folder
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }


    //IAction is a more generic version of a ViewResult... or vice versa?

    public ViewResult Index()
    {
        IEnumerable<MenuItem> MenuItem = _unitOfWork.MenuItem.List(null, null, "Category,FoodType");//WHERE, ORDERBY, JOIN
        return View(MenuItem);
    }


    //Upsert stuff
    private readonly IWebHostEnvironment _hostEnvironment;

    //Bind apparently solves some headaches by binding the model to our forum
    [BindProperty]
    public MenuItemVM MenuItemObj { get; set; }


    /*
     * We aren't going to have a separate edit/delete mode. Just upsert.
     * The id is optional, as you either pass it an id that gets edited, or you dont give it an id so it creates a new one.
     * */
    [HttpGet]
    public IActionResult Upsert(int? id) //optional id needed with edit mode vs create
    {
        var categories = _unitOfWork.Category.List();
        var foodTypes = _unitOfWork.FoodType.List();

        MenuItemObj = new MenuItemVM
        {
            MenuItem = new MenuItem(),
            //Something about populating the view model and fixing the... Id... bound... somewhere somehow
            CategoryList = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
            FoodTypeList = foodTypes.Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.Name })
        };

        if (id != null)
        {
            MenuItemObj.MenuItem = _unitOfWork.MenuItem.Get(u => u.Id == id, true);
            if (MenuItemObj == null)
            {
                return NotFound();
            }
        }

        return View(MenuItemObj);
    }


    [HttpPost]
    public IActionResult Upsert()
    {
        string webRootPath = _hostEnvironment.WebRootPath; //give root location
        var files = HttpContext.Request.Form.Files;

        if (!ModelState.IsValid)
        {
            return View();
        }

        if (MenuItemObj.MenuItem.Id == 0) //New Menu Item
        {
            if (files.Count > 0)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(webRootPath, @"images\menuitems\");
                var extension = Path.GetExtension(files[0].FileName);
                var fullpath = uploads + fileName + extension;

                using (var fileStream = System.IO.File.Create(fullpath))
                {
                    files[0].CopyTo(fileStream);
                }

                MenuItemObj.MenuItem.Image = @"\images\menuitems\" + fileName + extension;
            }

            _unitOfWork.MenuItem.Add(MenuItemObj.MenuItem);
        }
        else //update
        {
            var menuItemFromDb = _unitOfWork.MenuItem.Get(m => m.Id == MenuItemObj.MenuItem.Id, true);

            if (files.Count > 0)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(webRootPath, @"images\menuitems\");
                var extension = Path.GetExtension(files[0].FileName);

                if (menuItemFromDb.Image != null)
                {
                    var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath); //physically deleting the file
                    }
                }

                var fullpath = uploads + fileName + extension;
                using (var fileStream = System.IO.File.Create(fullpath))
                {
                    files[0].CopyTo(fileStream);
                }

                MenuItemObj.MenuItem.Image = @"\images\menuitems\" + fileName + extension;
            }
            else
            {
                MenuItemObj.MenuItem.Image = menuItemFromDb.Image;
            }

            _unitOfWork.MenuItem.Update(MenuItemObj.MenuItem);
        }

        _unitOfWork.Commit();
        return RedirectToAction("Index");
    }
}



