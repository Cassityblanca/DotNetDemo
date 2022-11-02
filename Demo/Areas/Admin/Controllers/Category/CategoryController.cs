using Demo.Interfaces;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)//Dependency Injection
    {
        _unitOfWork = unitOfWork;

    }

    public ViewResult Index()
    {
        IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();
        return View(objCategoryList);
    }





    [HttpGet]
    public ViewResult Create()
    {
        return View();

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Category obj)
    {
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            ModelState.AddModelError("name", "This Display order cannot exactly match the name");
        }

        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Add(obj); //internal add
            _unitOfWork.Commit(); //physical commit to DB table
            TempData["success"] = "Category created Successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }


    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
            return NotFound();

        //grab that Category from the DB itself

        var categoryFromDb = _unitOfWork.Category.Get(c => c.Id == id);

        if (categoryFromDb == null)
        {
            return NotFound();
        }

        return View(categoryFromDb);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Category obj)
    {
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            ModelState.AddModelError("name", "This Display order cannot exactly match the name");
        }



        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Update(obj);
            _unitOfWork.Commit();
            TempData["success"] = "Category updated Successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }
    [HttpGet]
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var categoryFromDb = _unitOfWork.Category.Get(c => c.Id == id);

        if (categoryFromDb == null)
        {
            return NotFound();
        }

        return View(categoryFromDb);

    }


    [HttpPost, ActionName("Delete")]     //can change the method name and just map the button on the html page to this ActionName

    public IActionResult DeletePost(int? id)
    {
        var obj = _unitOfWork.Category.Get(c => c.Id == id);
        if (obj == null)
        { return NotFound(); }

        _unitOfWork.Category.Delete(obj);
        _unitOfWork.Commit();
        TempData["success"] = "Category was deleted Successfully";
        return RedirectToAction("Index");
    }

}
