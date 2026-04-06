using System.Globalization;
using WorkForceKS.Data;
using WorkForceKS.Models;

namespace WorkForceKS.Repositories;

/// <summary>
/// CSV file-based implementation of IRepository&lt;Employee&gt;.
/// Handles missing files and malformed lines gracefully — never throws to caller.
/// </summary>
public class FileRepository : IRepository<Employee>
{
    private readonly string _filePath;
    private const string Header = "Id,Name,Position,Department,Salary,HiredAt";

    public FileRepository(string filePath = "data.csv")
    {
        _filePath = filePath;
        EnsureFileExists();
    }

    // ── Schema bootstrap ──────────────────────────────────────────────────────

    private void EnsureFileExists()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, Header + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            // If we cannot even create the file, log and continue — GetAll will return [].
            Console.Error.WriteLine($"  [FileRepository] Paralajmërim: nuk mund të krijohet '{_filePath}': {ex.Message}");
        }
    }

    // ── CRUD ──────────────────────────────────────────────────────────────────

    public List<Employee> GetAll()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("  [FileRepository] File nuk u gjet, po kthej listë bosh...");
                return new List<Employee>();
            }

            var lines = File.ReadAllLines(_filePath);
            var result = new List<Employee>();

            foreach (var line in lines.Skip(1)) // skip header
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var employee = ParseLine(line);
                if (employee is not null)
                    result.Add(employee);
            }

            return result;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"  [FileRepository] Gabim gjatë leximit të file: {ex.Message}");
            return new List<Employee>();
        }
    }

    public Employee? GetById(int id)
    {
        return GetAll().FirstOrDefault(e => e.Id == id);
    }

    public void Add(Employee employee)
    {
        var all = GetAll();
        employee.Id = all.Count > 0 ? all.Max(e => e.Id) + 1 : 1;
        all.Add(employee);
        Save(all);
    }

    public void Update(Employee updated)
    {
        var all   = GetAll();
        var index = all.FindIndex(e => e.Id == updated.Id);

        if (index == -1)
            throw new InvalidOperationException($"Punonjësi me ID {updated.Id} nuk u gjet.");

        all[index] = updated;
        Save(all);
    }

    public void Delete(int id)
    {
        var all     = GetAll();
        var initial = all.Count;
        all = all.Where(e => e.Id != id).ToList();

        if (all.Count == initial)
            throw new InvalidOperationException($"Punonjësi me ID {id} nuk u gjet.");

        Save(all);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void Save(List<Employee> employees)
    {
        try
        {
            var lines = new List<string> { Header };
            lines.AddRange(employees.Select(Serialize));
            File.WriteAllLines(_filePath, lines);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"Gabim gjatë shkrimit të file: {ex.Message}", ex);
        }
    }

    private static string Serialize(Employee e) =>
        string.Join(",",
            e.Id,
            EscapeCsv(e.Name),
            EscapeCsv(e.Position),
            EscapeCsv(e.Department),
            e.Salary.ToString(CultureInfo.InvariantCulture),
            e.HiredAt.ToString("yyyy-MM-dd"));

    private static Employee? ParseLine(string line)
    {
        try
        {
            var parts = line.Split(',');
            if (parts.Length < 6) return null;

            return new Employee
            {
                Id         = int.Parse(parts[0].Trim()),
                Name       = parts[1].Trim(),
                Position   = parts[2].Trim(),
                Department = parts[3].Trim(),
                Salary     = decimal.Parse(parts[4].Trim(), CultureInfo.InvariantCulture),
                HiredAt    = DateTime.Parse(parts[5].Trim()),
            };
        }
        catch
        {
            // Skip malformed lines silently
            return null;
        }
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
