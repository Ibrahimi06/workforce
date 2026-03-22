using Models;

namespace Data;

public class FileRepository : IRepository<Employee>
{
    private List<Employee> employees = new();
    private string filePath;

    public FileRepository(string path)
    {
        filePath = path;
        Load();
    }

    private void Load()
    {
        if (!File.Exists(filePath)) return;

        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            var parts = line.Split(",");
            employees.Add(new Employee
            {
                Id = int.Parse(parts[0]),
                Name = parts[1],
                Position = parts[2]
            });
        }
    }

    public List<Employee> GetAll() => employees;

    public Employee GetById(int id)
        => employees.FirstOrDefault(e => e.Id == id);

    public void Add(Employee entity)
    {
        employees.Add(entity);
    }

    public void Save()
    {
        var lines = employees.Select(e =>
            $"{e.Id},{e.Name},{e.Position}");

        File.WriteAllLines(filePath, lines);
    }
}