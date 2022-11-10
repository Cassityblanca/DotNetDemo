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


    //////Upsert stuff
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
    public IActionResult Upsert() //note the lack of paramters. The menu item from the form comes from the [BindProperty] declaration.
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
                var uploads = Path.Combine(webRootPath, @"images\menuitems\"); //appends webrootpath (the path of whatever is running our server) + images\menuitems
                var extension = Path.GetExtension(files[0].FileName);  //^^the @ sign means 'this string is literal, no escapes'
                var fullpath = uploads + fileName + extension; //full path is their file name, our random rename, and the extension.
                //if we had multiple files, we could loop through the files array rather than just files[0]


                using (var fileStream = System.IO.File.Create(fullpath)) //stream the image as a binary file
                {
                    files[0].CopyTo(fileStream);
                }

                //the database has an image property, now save that property as a string as a path to the image. So the IMAGE ITSELF is NOT saved to the value image, the PATH to the image is saved.
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

    [HttpGet]
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var menuItemFromDb = _unitOfWork.MenuItem.Get(m => m.Id == id);

        if (menuItemFromDb == null)
        {
            return NotFound();
        }

        return View(menuItemFromDb);

    }


    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var menuItemFromDb = _unitOfWork.MenuItem.Get(m => m.Id == id);
        if (menuItemFromDb == null)
        { return NotFound(); }

        string webRootPath = _hostEnvironment.WebRootPath; //give root location
        string fileName = Guid.NewGuid().ToString();
        var uploads = Path.Combine(webRootPath, @"images\menuitems\");
        var extension = Path.GetExtension(fileName);

        if (menuItemFromDb.Image != null)
        {
            var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath); //physically deleting the file
            }
        }

        _unitOfWork.MenuItem.Delete(menuItemFromDb);
        _unitOfWork.Commit();
        return RedirectToAction("Index");
    }

}



