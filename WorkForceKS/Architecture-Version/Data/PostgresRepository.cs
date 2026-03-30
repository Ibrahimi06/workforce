using Npgsql;
using WorkForceKS.Models;

namespace WorkForceKS.Data;

/// <summary>
/// PostgreSQL implementation of IRepository&lt;Employee&gt;.
/// Handles all DB interaction via Npgsql — no ORM, raw SQL for transparency.
/// </summary>
public class PostgresRepository : IRepository<Employee>
{
    private readonly string _connectionString;

    public PostgresRepository(string connectionString)
    {
        _connectionString = connectionString;
        EnsureTableExists();
    }

    // ── Schema bootstrap ──────────────────────────────────────────────────────

    private void EnsureTableExists()
    {
        using var conn = Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS employees (
                id          SERIAL PRIMARY KEY,
                name        VARCHAR(150) NOT NULL,
                position    VARCHAR(100) NOT NULL,
                department  VARCHAR(100) NOT NULL,
                salary      NUMERIC(12,2) NOT NULL CHECK (salary > 0),
                hired_at    DATE NOT NULL DEFAULT CURRENT_DATE
            );
            """;
        cmd.ExecuteNonQuery();
    }

    // ── Seed (called once if table is empty) ──────────────────────────────────

    public void SeedIfEmpty()
    {
        using var conn = Open();
        using var check = conn.CreateCommand();
        check.CommandText = "SELECT COUNT(*) FROM employees;";
        var count = (long)(check.ExecuteScalar() ?? 0L);
        if (count > 0) return;

        var seed = new List<Employee>
        {
            new() { Name = "Artan Berisha",   Position = "Menaxher",        Department = "HR",          Salary = 1800m, HiredAt = new DateTime(2020, 3, 1)  },
            new() { Name = "Vjosa Gashi",     Position = "Zhvillues",        Department = "IT",          Salary = 1500m, HiredAt = new DateTime(2021, 6, 15) },
            new() { Name = "Blerim Krasniqi", Position = "Analista",         Department = "Financa",     Salary = 1350m, HiredAt = new DateTime(2019, 1, 10) },
            new() { Name = "Drita Morina",    Position = "Dizajner UI/UX",   Department = "IT",          Salary = 1200m, HiredAt = new DateTime(2022, 9, 1)  },
            new() { Name = "Fatos Hyseni",    Position = "Kontabilist",      Department = "Financa",     Salary = 1100m, HiredAt = new DateTime(2018, 4, 20) },
            new() { Name = "Lirie Osmani",    Position = "Asistent Admin",   Department = "Administrim", Salary =  950m, HiredAt = new DateTime(2023, 2, 1)  },
        };

        foreach (var e in seed) Add(e);
    }

    // ── CRUD ──────────────────────────────────────────────────────────────────

    public List<Employee> GetAll()
    {
        var result = new List<Employee>();
        using var conn = Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = "SELECT id, name, position, department, salary, hired_at FROM employees ORDER BY id;";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(Map(reader));

        return result;
    }

    public Employee? GetById(int id)
    {
        using var conn = Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = "SELECT id, name, position, department, salary, hired_at FROM employees WHERE id = @id;";
        cmd.Parameters.AddWithValue("id", id);

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public void Add(Employee e)
    {
        using var conn = Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO employees (name, position, department, salary, hired_at)
            VALUES (@name, @position, @department, @salary, @hired_at)
            RETURNING id;
            """;
        cmd.Parameters.AddWithValue("name",       e.Name);
        cmd.Parameters.AddWithValue("position",   e.Position);
        cmd.Parameters.AddWithValue("department", e.Department);
        cmd.Parameters.AddWithValue("salary",     e.Salary);
        cmd.Parameters.AddWithValue("hired_at",   e.HiredAt.Date);

        e.Id = Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Employee e)
    {
        using var conn = Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE employees
            SET name = @name, position = @position, department = @department,
                salary = @salary, hired_at = @hired_at
            WHERE id = @id;
            """;
        cmd.Parameters.AddWithValue("id",         e.Id);
        cmd.Parameters.AddWithValue("name",       e.Name);
        cmd.Parameters.AddWithValue("position",   e.Position);
        cmd.Parameters.AddWithValue("department", e.Department);
        cmd.Parameters.AddWithValue("salary",     e.Salary);
        cmd.Parameters.AddWithValue("hired_at",   e.HiredAt.Date);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM employees WHERE id = @id;";
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private NpgsqlConnection Open()
    {
        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        return conn;
    }

    private static Employee Map(NpgsqlDataReader r) => new()
    {
        Id         = r.GetInt32(0),
        Name       = r.GetString(1),
        Position   = r.GetString(2),
        Department = r.GetString(3),
        Salary     = r.GetDecimal(4),
        HiredAt    = r.GetDateTime(5),
    };
}
