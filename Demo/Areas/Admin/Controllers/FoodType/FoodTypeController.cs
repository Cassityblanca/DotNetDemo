using Demo.Interfaces;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

[Area("Admin")]
public class FoodTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public FoodTypeController(IUnitOfWork unitOfWork)//Dependency Injection
    {
        _unitOfWork = unitOfWork;

    }

    public ViewResult Index()
    {
        IEnumerable<FoodType> objFoodTypeList = _unitOfWork.FoodType.GetAll();
        return View(objFoodTypeList);
    }





    [HttpGet]
    public ViewResult Create()
    {
        return View();

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(FoodType obj)
    {

        if (ModelState.IsValid)
        {
            _unitOfWork.FoodType.Add(obj); //internal add
            _unitOfWork.Commit(); //physical commit to DB table
            TempData["success"] = "FoodType created Successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }


    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
            return NotFound();

        //grab that FoodType from the DB itself

        var FoodTypeFromDb = _unitOfWork.FoodType.Get(c => c.Id == id);

        if (FoodTypeFromDb == null)
        {
            return NotFound();
        }

        return View(FoodTypeFromDb);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(FoodType obj)
    {



        if (ModelState.IsValid)
        {
            _unitOfWork.FoodType.Update(obj);
            _unitOfWork.Commit();
            TempData["success"] = "FoodType updated Successfully";
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

        var FoodTypeFromDb = _unitOfWork.FoodType.Get(c => c.Id == id);

        if (FoodTypeFromDb == null)
        {
            return NotFound();
        }

        return View(FoodTypeFromDb);

    }


    [HttpPost, ActionName("Delete")]     //can change the method name and just map the button on the html page to this ActionName

    public IActionResult DeletePost(int? id)
    {
        var obj = _unitOfWork.FoodType.Get(c => c.Id == id);
        if (obj == null)
        { return NotFound(); }

        _unitOfWork.FoodType.Delete(obj);
        _unitOfWork.Commit();
        TempData["success"] = "FoodType was deleted Successfully";
        return RedirectToAction("Index");
    }

}
