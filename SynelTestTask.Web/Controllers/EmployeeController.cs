using Microsoft.AspNetCore.Mvc;
using SynelTestTask.DataAccess.Repository.IRepository;
using SynelTestTask.Models;

namespace SynelTestTask.Web.Controllers;
public class EmployeeController : Controller
{
    private readonly IEmployeeRepository employeeRepository;

    public EmployeeController(IEmployeeRepository employeeRepository)
    {
        this.employeeRepository = employeeRepository;
    }

    // Action for displaying all employees
    public ActionResult Index()
    {
        List<Employee> employees = employeeRepository.GetAll().ToList();
        return View("Index", employees);
    }

    // Action for handling CSV file
    [HttpPost]
    public IActionResult Index(IFormFile postedFile)
    {
        int processedRows = employeeRepository.ReadFromCSV(postedFile);
        List<Employee> employees = employeeRepository.GetAll().ToList();
        TempData["success"] = $"{processedRows} rows successfully processed";
        return View("Index", employees);
    }

    // Action for display the edit form for a specific employee
    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }
        Employee employee = employeeRepository.Get(v => v.Id == id);

        if (employee == null){
            return NotFound();
        }

        return View("Edit", employee);
    }

    // Action for handling the form submission for updating en employee
    [HttpPost]
    public IActionResult Edit(Employee employee)
    {
        if(ModelState.IsValid)
        {
            employeeRepository.Update(employee);
            employeeRepository.Save();
            TempData["success"] = "Employee updated successfully";
            return RedirectToAction("Index");
        }
        return View("Edit", employee);
    }
}