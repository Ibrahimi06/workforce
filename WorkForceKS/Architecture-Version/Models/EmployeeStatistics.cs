namespace WorkForceKS.Models;

/// <summary>
/// Holds computed statistics for a group of employees.
/// </summary>
public class EmployeeStatistics
{
    public int     Count          { get; init; }
    public decimal TotalSalary    { get; init; }
    public decimal AverageSalary  { get; init; }
    public decimal MaxSalary      { get; init; }
    public decimal MinSalary      { get; init; }
    public string  TopEarner      { get; init; } = string.Empty;
    public string  FilterApplied  { get; init; } = "Të gjithë";

    public override string ToString() =>
        $"Statistika [{FilterApplied}]: " +
        $"Gjithsej={Count}, " +
        $"Total=€{TotalSalary:N2}, " +
        $"Mesatare=€{AverageSalary:N2}, " +
        $"Max=€{MaxSalary:N2}, " +
        $"Min=€{MinSalary:N2}, " +
        $"Paguesi më i lartë={TopEarner}";
}
