using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SynelTestTask.DataAccess.Repository.IRepository;
using SynelTestTask.Models;
using SynelTestTask.Web.Controllers;
using Xunit;

namespace SynelTestTask.Test;

public class EmployeeControllerTest
{
    private readonly Mock<IEmployeeRepository> mockRepo;
    private readonly EmployeeController employeeController;

    public EmployeeControllerTest()
    {
        mockRepo = new Mock<IEmployeeRepository>();
        employeeController = new EmployeeController(mockRepo.Object);
    }

    [Fact]
    public void Index_ReturnsWithEmployees()
    {
        // Arrange
        mockRepo.Setup(r => r.GetAll())
                .Returns(new List<Employee>());
        
        // Act
        var result = employeeController.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result.ViewName);
        Assert.IsType<List<Employee>>(result.Model);
    }

    [Fact]
    public void IndexPost_ReturnsViewEmployeesAfterReadingCSV()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        int processedRows = 5;
        mockRepo.Setup(r => r.ReadFromCSV(It.IsAny<IFormFile>())).Returns(processedRows);

        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        employeeController.TempData = tempData;

        // Act
        var result = employeeController.Index(fileMock.Object) as ViewResult;

        // Assert
        mockRepo.Verify(r => r.ReadFromCSV(fileMock.Object), Times.Once);

        Assert.NotNull(result);
        Assert.Equal("Index", result.ViewName);
        Assert.IsType<List<Employee>>(result.Model);
        Assert.Equal($"{processedRows} rows successfully processed", tempData["success"]);
    }

    [Fact]
    public void EditGet_ReturnsViewWithEmployee()
    {
        // Arrange
        int employeeId = 1;
        mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Employee, bool>>>()))
                .Returns(
                    new Employee 
                    {
                        Id = employeeId
                    });
        
        // Act
        var result = employeeController.Edit(employeeId) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Edit", result.ViewName);
        Assert.IsType<Employee>(result.Model);
    }

    [Fact]
    public void EditPost_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var validEmployee = new Employee
        {
            Id = 1,
            PayrollNumber = "QWO0R2",
            Forenames = "Sarvar",
            Surname = "Azodov"
        };

        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        employeeController.TempData = tempData;

        // Act
        var result = employeeController.Edit(validEmployee) as RedirectToActionResult;

        // Assert
        mockRepo.Verify(r => r.Update(validEmployee), Times.Once);
        mockRepo.Verify(r => r.Save(), Times.Once);

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Equal("Employee updated successfully", employeeController.TempData["success"]);
    }

    [Fact]
    public void EditPost_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        employeeController.ModelState.AddModelError("Name", "Name is required");
        var invalidEmployee = new Employee
        {
            Id = 1
        };

        // Act
        var result = employeeController.Edit(invalidEmployee) as ViewResult;

        // Assert
        mockRepo.Verify(r => r.Update(It.IsAny<Employee>()), Times.Never);
        mockRepo.Verify(r => r.Save(), Times.Never);

        Assert.NotNull(result);
        Assert.Equal("Edit", result.ViewName);
        Assert.IsType<Employee>(result.Model);
    }
}