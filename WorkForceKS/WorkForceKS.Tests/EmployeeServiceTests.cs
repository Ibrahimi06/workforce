using WorkForceKS.Services;
using WorkForceKS.Models;
using WorkForceKS.Tests.Fakes;
using Xunit;

namespace WorkForceKS.Tests;

/// <summary>
/// Unit tests for EmployeeService.
/// Sprint 2: covers Statistics feature + edge cases + error handling.
/// </summary>
public class EmployeeServiceTests
{
    // ── Factory helpers ───────────────────────────────────────────────────────

    private static EmployeeService CreateService() =>
        new EmployeeService(new InMemoryRepository());

    private static EmployeeService CreateServiceWithData()
    {
        var repo    = new InMemoryRepository();
        var service = new EmployeeService(repo);

        service.Add("Artan Berisha",   "Menaxher",      "HR",          1800m, new DateTime(2020, 3, 1));
        service.Add("Vjosa Gashi",     "Zhvillues",      "IT",          1500m, new DateTime(2021, 6, 15));
        service.Add("Blerim Krasniqi", "Analista",       "Financa",     1350m, new DateTime(2019, 1, 10));
        service.Add("Drita Morina",    "Dizajner UI/UX", "IT",          1200m, new DateTime(2022, 9, 1));
        service.Add("Fatos Hyseni",    "Kontabilist",    "Financa",     1100m, new DateTime(2018, 4, 20));

        return service;
    }

    // ── CRUD — Shtim ──────────────────────────────────────────────────────────

    [Fact]
    public void Add_ValidEmployee_ReturnsEmployeeWithId()
    {
        var service = CreateService();

        var result = service.Add("Artan Berisha", "Menaxher", "HR", 1800m, DateTime.Today);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Artan Berisha", result.Name);
    }

    [Fact]
    public void Add_EmptyName_ThrowsArgumentException()
    {
        var service = CreateService();

        var ex = Assert.Throws<ArgumentException>(() =>
            service.Add("", "Menaxher", "HR", 1800m, DateTime.Today));

        Assert.Contains("Emri", ex.Message);
    }

    [Fact]
    public void Add_ZeroSalary_ThrowsArgumentException()
    {
        var service = CreateService();

        var ex = Assert.Throws<ArgumentException>(() =>
            service.Add("Artan Berisha", "Menaxher", "HR", 0m, DateTime.Today));

        Assert.Contains("Paga", ex.Message);
    }

    [Fact]
    public void Add_NegativeSalary_ThrowsArgumentException()
    {
        var service = CreateService();

        var ex = Assert.Throws<ArgumentException>(() =>
            service.Add("Artan Berisha", "Menaxher", "HR", -500m, DateTime.Today));

        Assert.Contains("Paga", ex.Message);
    }

    // ── CRUD — Kërkim ─────────────────────────────────────────────────────────

    [Fact]
    public void FindById_ExistingEmployee_ReturnsEmployee()
    {
        var service = CreateServiceWithData();

        var result = service.FindById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Artan Berisha", result.Name);
    }

    [Fact]
    public void FindById_NonExistingId_ReturnsNull()
    {
        var service = CreateServiceWithData();

        var result = service.FindById(999);

        Assert.Null(result);
    }

    [Fact]
    public void FindById_ZeroId_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.FindById(0));
    }

    [Fact]
    public void FindById_NegativeId_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.FindById(-1));
    }

    // ── CRUD — Listim me filtrim ───────────────────────────────────────────────

    [Fact]
    public void List_NoFilter_ReturnsAllEmployees()
    {
        var service = CreateServiceWithData();

        var result = service.List();

        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void List_FilterByDepartment_ReturnsOnlyMatchingEmployees()
    {
        var service = CreateServiceWithData();

        var result = service.List(department: "IT");

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Equal("IT", e.Department));
    }

    [Fact]
    public void List_FilterByMinSalary_ReturnsOnlyHighEarners()
    {
        var service = CreateServiceWithData();

        var result = service.List(minSalary: 1400m);

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.True(e.Salary >= 1400m));
    }

    [Fact]
    public void List_DepartmentCaseInsensitive_ReturnsResults()
    {
        var service = CreateServiceWithData();

        var result = service.List(department: "it");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void List_NonExistingDepartment_ReturnsEmptyList()
    {
        var service = CreateServiceWithData();

        var result = service.List(department: "NukEkziston");

        Assert.Empty(result);
    }

    // ── CRUD — Fshirje ────────────────────────────────────────────────────────

    [Fact]
    public void Delete_ExistingEmployee_RemovesFromList()
    {
        var service = CreateServiceWithData();
        var before  = service.List().Count;

        service.Delete(1);

        var after = service.List().Count;
        Assert.Equal(before - 1, after);
        Assert.Null(service.FindById(1));
    }

    [Fact]
    public void Delete_NonExistingId_ThrowsInvalidOperationException()
    {
        var service = CreateServiceWithData();

        Assert.Throws<InvalidOperationException>(() => service.Delete(999));
    }

    // ── SPRINT 2 — Statistika ─────────────────────────────────────────────────

    [Fact]
    public void GetStatistics_AllEmployees_ReturnsCorrectCount()
    {
        var service = CreateServiceWithData();

        var stats = service.GetStatistics();

        Assert.NotNull(stats);
        Assert.Equal(5, stats.Count);
    }

    [Fact]
    public void GetStatistics_AllEmployees_ReturnsCorrectTotal()
    {
        var service = CreateServiceWithData();

        var stats = service.GetStatistics();

        Assert.NotNull(stats);
        // 1800 + 1500 + 1350 + 1200 + 1100 = 6950
        Assert.Equal(6950m, stats.TotalSalary);
    }

    [Fact]
    public void GetStatistics_AllEmployees_ReturnsCorrectMax()
    {
        var service = CreateServiceWithData();

        var stats = service.GetStatistics();

        Assert.NotNull(stats);
        Assert.Equal(1800m, stats.MaxSalary);
        Assert.Equal("Artan Berisha", stats.TopEarner);
    }

    [Fact]
    public void GetStatistics_AllEmployees_ReturnsCorrectMin()
    {
        var service = CreateServiceWithData();

        var stats = service.GetStatistics();

        Assert.NotNull(stats);
        Assert.Equal(1100m, stats.MinSalary);
    }

    [Fact]
    public void GetStatistics_FilteredByDepartment_ReturnsCorrectStats()
    {
        var service = CreateServiceWithData();

        var stats = service.GetStatistics(department: "IT");

        Assert.NotNull(stats);
        Assert.Equal(2, stats.Count);
        Assert.Equal(1500m, stats.MaxSalary);
        Assert.Equal(1200m, stats.MinSalary);
        Assert.Equal(1350m, stats.AverageSalary);
    }

    [Fact]
    public void GetStatistics_EmptyRepository_ReturnsNull()
    {
        var service = CreateService();

        var stats = service.GetStatistics();

        Assert.Null(stats);
    }

    [Fact]
    public void GetStatistics_NonExistingDepartment_ReturnsNull()
    {
        var service = CreateServiceWithData();

        var stats = service.GetStatistics(department: "NukEkziston");

        Assert.Null(stats);
    }

    // ── Raste kufitare ────────────────────────────────────────────────────────

    [Fact]
    public void Add_MultipleEmployees_IdsAreUnique()
    {
        var service = CreateService();

        var e1 = service.Add("Punonjesi 1", "Pozita", "Dept", 1000m, DateTime.Today);
        var e2 = service.Add("Punonjesi 2", "Pozita", "Dept", 1000m, DateTime.Today);
        var e3 = service.Add("Punonjesi 3", "Pozita", "Dept", 1000m, DateTime.Today);

        var ids = new[] { e1.Id, e2.Id, e3.Id };
        Assert.Equal(ids.Length, ids.Distinct().Count());
    }

    [Fact]
    public void Add_WhitespaceOnlyName_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() =>
            service.Add("   ", "Pozita", "Dept", 1000m, DateTime.Today));
    }

    [Fact]
    public void GetStatistics_SingleEmployee_AverageEqualsOnlySalary()
    {
        var service = CreateService();
        service.Add("Testi", "Dev", "IT", 1234m, DateTime.Today);

        var stats = service.GetStatistics();

        Assert.NotNull(stats);
        Assert.Equal(1234m, stats.AverageSalary);
        Assert.Equal(1234m, stats.MaxSalary);
        Assert.Equal(1234m, stats.MinSalary);
    }
}
