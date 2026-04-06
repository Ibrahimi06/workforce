using WorkForceKS.Data;
using WorkForceKS.Models;

namespace WorkForceKS.Tests.Fakes;

/// <summary>
/// In-memory IRepository implementation used only in unit tests.
/// Avoids file I/O so tests are fast, isolated, and repeatable.
/// </summary>
public class InMemoryRepository : IRepository<Employee>
{
    private readonly List<Employee> _store = new();
    private int _nextId = 1;

    public List<Employee> GetAll() =>
        _store.Select(Clone).ToList();

    public Employee? GetById(int id) =>
        _store.FirstOrDefault(e => e.Id == id) is { } e ? Clone(e) : null;

    public void Add(Employee employee)
    {
        employee.Id = _nextId++;
        _store.Add(Clone(employee));
    }

    public void Update(Employee updated)
    {
        var index = _store.FindIndex(e => e.Id == updated.Id);
        if (index == -1)
            throw new InvalidOperationException($"ID {updated.Id} nuk u gjet.");
        _store[index] = Clone(updated);
    }

    public void Delete(int id)
    {
        var index = _store.FindIndex(e => e.Id == id);
        if (index == -1)
            throw new InvalidOperationException($"ID {id} nuk u gjet.");
        _store.RemoveAt(index);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Employee Clone(Employee e) => new()
    {
        Id         = e.Id,
        Name       = e.Name,
        Position   = e.Position,
        Department = e.Department,
        Salary     = e.Salary,
        HiredAt    = e.HiredAt,
    };
}
