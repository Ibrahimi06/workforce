using Models;
using Data;

namespace Services;

public class EmployeeService
{
    private readonly IRepository<Employee> repo;

    public EmployeeService(IRepository<Employee> repository)
    {
        repo = repository;
    }

    public List<Employee> GetEmployees()
    {
        return repo.GetAll();
    }

    public void AddEmployee(int id, string name, string position)
    {
        repo.Add(new Employee
        {
            Id = id,
            Name = name,
            Position = position
        });

        repo.Save();
    }
}
