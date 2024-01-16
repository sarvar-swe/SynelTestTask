using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SynelTestTask.DataAccess.Data;
using SynelTestTask.DataAccess.Repository.IRepository;
using SynelTestTask.Models;

namespace SynelTestTask.DataAccess.Repository;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    private readonly IHostingEnvironment hostingEnvironment;
    private readonly AppDbContext dbContext;

    public EmployeeRepository(
        IHostingEnvironment hostingEnvironment,
        AppDbContext dbContext) : base(dbContext)
    {
        this.hostingEnvironment = hostingEnvironment;
        this.dbContext = dbContext;
    }

    public void Save()
    {
        dbContext.SaveChanges();
    }

    public void Update(Employee employee)
    {
        dbContext.Employees.Update(employee);
    }

    public int ReadFromCSV(IFormFile file)
    {
        string path = Path.Combine(hostingEnvironment.WebRootPath, "Uploads");
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string fileName = Path.GetFileName(file.FileName);
        string filePath = Path.Combine(path, fileName);
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        List<Employee> employees = File
            .ReadAllLines(filePath)
            .Skip(1)
            .Select(v => ParseCsv(v))
            .ToList();

        AddRecordsToDb(employees);

        return employees.Count;
    }

    private void AddRecordsToDb(List<Employee> employees)
    {
        foreach (var employee in employees)
        {
            dbSet.Add(employee);
        }
        Save();
    }

    private static Employee ParseCsv(string csvLine)
    {
        string[] values = csvLine.Split(',');
        Employee employee = new Employee();
        employee.PayrollNumber = values[0];
        employee.Forenames = values[1];
        employee.Surname = values[2];
        employee.DateOfBirth = DateOnly.ParseExact(values[3], "d/M/yyyy", CultureInfo.InvariantCulture);
        employee.Telephone = values[4];
        employee.Mobile = values[5];
        employee.Address = values[6];
        employee.Address2 = values[7];
        employee.Postcode = values[8];
        employee.EmailHome = values[9];
        employee.StartDate = DateOnly.ParseExact(values[10].TrimEnd(), "d/M/yyyy", CultureInfo.InvariantCulture);

        return employee;
    }
}