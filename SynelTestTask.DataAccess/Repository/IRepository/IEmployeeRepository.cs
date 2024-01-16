using Microsoft.AspNetCore.Http;
using SynelTestTask.Models;

namespace SynelTestTask.DataAccess.Repository.IRepository;

public interface IEmployeeRepository : IRepository<Employee>
{
    void Update(Employee employee);
    void Save();
    int ReadFromCSV(IFormFile fromFile);
}