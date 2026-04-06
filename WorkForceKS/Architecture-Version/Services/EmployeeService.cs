using WorkForceKS.Data;
using WorkForceKS.Models;

namespace WorkForceKS.Services;

/// <summary>
/// Business logic layer for Employee management.
/// Sprint 2: added Statistics feature and comprehensive error handling.
/// Flow: UI → Service → Repository
/// </summary>
public class EmployeeService
{
    private readonly IRepository<Employee> _repo;

    public EmployeeService(IRepository<Employee> repository)
    {
        _repo = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    // ── 1. Listo (me filtrim) ─────────────────────────────────────────────────

    /// <summary>
    /// Returns all employees, optionally filtered by department and/or minimum salary.
    /// </summary>
    public List<Employee> List(string? department = null, decimal? minSalary = null)
    {
        var employees = _repo.GetAll();

        if (!string.IsNullOrWhiteSpace(department))
            employees = employees
                .Where(e => e.Department.Contains(department.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (minSalary.HasValue)
        {
            if (minSalary.Value < 0)
                throw new ArgumentException("Paga minimale nuk mund të jetë negative.");

            employees = employees
                .Where(e => e.Salary >= minSalary.Value)
                .ToList();
        }

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
    /// Validates and adds a new employee. Throws ArgumentException on invalid input.
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

    // ── 4. Ndrysho ────────────────────────────────────────────────────────────

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

    // ── 5. Fshi ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Deletes an employee by ID. Throws if not found.
    /// </summary>
    public void Delete(int id)
    {
        var existing = _repo.GetById(id)
            ?? throw new InvalidOperationException($"Punonjësi me ID {id} nuk ekziston.");

        _repo.Delete(existing.Id);
    }

    // ── 6. SPRINT 2 — Statistika ──────────────────────────────────────────────

    /// <summary>
    /// Calculates statistics (total, average, max, min salary, count, top earner)
    /// for all employees or filtered by department.
    /// Returns null if no employees match the filter.
    /// </summary>
    public EmployeeStatistics? GetStatistics(string? department = null)
    {
        var employees = List(department);

        if (employees.Count == 0)
            return null;

        var topEarner = employees
            .OrderByDescending(e => e.Salary)
            .First();

        return new EmployeeStatistics
        {
            Count         = employees.Count,
            TotalSalary   = employees.Sum(e => e.Salary),
            AverageSalary = employees.Average(e => e.Salary),
            MaxSalary     = employees.Max(e => e.Salary),
            MinSalary     = employees.Min(e => e.Salary),
            TopEarner     = topEarner.Name,
            FilterApplied = string.IsNullOrWhiteSpace(department)
                                ? "Të gjithë departamentet"
                                : department.Trim(),
        };
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
