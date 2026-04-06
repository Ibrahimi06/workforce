namespace WorkForceKS.Models;

/// <summary>
/// Represents an employee in the WorkForce KS system.
/// </summary>
public class Employee
{
    public int     Id         { get; set; }
    public string  Name       { get; set; } = string.Empty;
    public string  Position   { get; set; } = string.Empty;
    public string  Department { get; set; } = string.Empty;
    public decimal Salary     { get; set; }
    public DateTime HiredAt   { get; set; }

    public override string ToString() =>
        $"[{Id}] {Name} | {Position} | {Department} | €{Salary:N2} | Punësuar: {HiredAt:yyyy-MM-dd}";
}
