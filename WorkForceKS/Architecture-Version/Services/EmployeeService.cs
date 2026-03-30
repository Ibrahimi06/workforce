using WorkForceKS.Data;
using WorkForceKS.Models;

namespace WorkForceKS.Services;

/// <summary>
/// Business logic layer for Employee management.
/// Receives IRepository via constructor injection — no direct DB coupling.
/// </summary>
public class EmployeeService
{
    private readonly IRepository<Employee> _repo;

    public EmployeeService(IRepository<Employee> repository)
    {
        _repo = repository;
    }

    // ── 1. Listo (me filtrim) ─────────────────────────────────────────────────

    /// <summary>
    /// Returns all employees, optionally filtered by department and/or min salary.
    /// </summary>
    public List<Employee> List(string? department = null, decimal? minSalary = null)
    {
        var employees = _repo.GetAll();

        if (!string.IsNullOrWhiteSpace(department))
            employees = employees
                .Where(e => e.Department.Contains(department, StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (minSalary.HasValue)
            employees = employees
                .Where(e => e.Salary >= minSalary.Value)
                .ToList();

        return employees;
    }

    // ── 2. Gjej sipas ID ──────────────────────────────────────────────────────

    /// <summary>
    /// Finds a single employee by ID. Returns null if not found.
    /// </summary>
    public Employee? FindById(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID duhet të jetë numër pozitiv.");

        return _repo.GetById(id);
    }

    // ── 3. Shto (me validim) ──────────────────────────────────────────────────

    /// <summary>
    /// Validates and adds a new employee. Throws on invalid input.
    /// </summary>
    public Employee Add(string name, string position, string department,
                        decimal salary, DateTime hiredAt)
    {
        Validate(name, position, department, salary);

        var employee = new Employee
        {
            Name       = name.Trim(),
            Position   = position.Trim(),
            Department = department.Trim(),
            Salary     = salary,
            HiredAt    = hiredAt,
        };

        _repo.Add(employee);
        return employee;
    }

    // ── Update ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Updates an existing employee. Throws if not found or invalid data.
    /// </summary>
    public Employee Update(int id, string name, string position,
                           string department, decimal salary, DateTime hiredAt)
    {
        Validate(name, position, department, salary);

        var existing = _repo.GetById(id)
            ?? throw new InvalidOperationException($"Punonjësi me ID {id} nuk u gjet.");

        existing.Name       = name.Trim();
        existing.Position   = position.Trim();
        existing.Department = department.Trim();
        existing.Salary     = salary;
        existing.HiredAt    = hiredAt;

        _repo.Update(existing);
        return existing;
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Deletes an employee by ID. Throws if not found.
    /// </summary>
    public void Delete(int id)
    {
        var existing = _repo.GetById(id)
            ?? throw new InvalidOperationException($"Punonjësi me ID {id} nuk ekziston.");

        _repo.Delete(id);
    }

    // ── Private validation ────────────────────────────────────────────────────

    private static void Validate(string name, string position,
                                  string department, decimal salary)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("Emri nuk mund të jetë bosh.");

        if (string.IsNullOrWhiteSpace(position))
            errors.Add("Pozita nuk mund të jetë bosh.");

        if (string.IsNullOrWhiteSpace(department))
            errors.Add("Departamenti nuk mund të jetë bosh.");

        if (salary <= 0)
            errors.Add("Paga duhet të jetë më e madhe se 0.");

        if (errors.Count > 0)
            throw new ArgumentException(string.Join("\n  → ", errors));
    }
}
