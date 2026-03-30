using WorkForceKS.Services;
using WorkForceKS.Models;

namespace WorkForceKS.UI;

/// <summary>
/// Console-based user interface.
/// Flow: UI → Service → Repository → PostgreSQL
/// </summary>
public class ConsoleUI
{
    private readonly EmployeeService _service;

    public ConsoleUI(EmployeeService service)
    {
        _service = service;
    }

    public void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Banner();

        while (true)
        {
            PrintMenu();
            var choice = Prompt("Zgjidhni një opsion").Trim();

            try
            {
                switch (choice)
                {
                    case "1": ShowAll();        break;
                    case "2": ShowFiltered();   break;
                    case "3": FindById();       break;
                    case "4": AddEmployee();    break;
                    case "5": UpdateEmployee(); break;
                    case "6": DeleteEmployee(); break;
                    case "0":
                        Console.WriteLine("\n  Mirupafshim! 👋\n");
                        return;
                    default:
                        Warn("Opsion i pavlefshëm. Provoni sërish.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

            Console.WriteLine("\nShtypni Enter për të vazhduar...");
            Console.ReadLine();
        }
    }

    // ── Menu ─────────────────────────────────────────────────────────────────

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("  ┌─────────────────────────────────┐");
        Console.WriteLine("  │         WORKFORCE KS            │");
        Console.WriteLine("  ├─────────────────────────────────┤");
        Console.WriteLine("  │  1. Listo të gjithë punonjësit  │");
        Console.WriteLine("  │  2. Listo me filtrim            │");
        Console.WriteLine("  │  3. Kërko sipas ID              │");
        Console.WriteLine("  │  4. Shto punonjës               │");
        Console.WriteLine("  │  5. Ndrysho punonjës            │");
        Console.WriteLine("  │  6. Fshi punonjës               │");
        Console.WriteLine("  │  0. Dil                         │");
        Console.WriteLine("  └─────────────────────────────────┘");
    }

    // ── 1 — Show all ─────────────────────────────────────────────────────────

    private void ShowAll()
    {
        var list = _service.List();
        PrintTable(list);
    }

    // ── 2 — Filtered list ────────────────────────────────────────────────────

    private void ShowFiltered()
    {
        var dept     = Prompt("Departamenti (lër bosh për të gjithë)");
        var salaryTx = Prompt("Paga minimale (lër bosh për asnjë limit)");

        decimal? minSalary = null;
        if (!string.IsNullOrWhiteSpace(salaryTx))
            minSalary = decimal.Parse(salaryTx);

        var list = _service.List(
            string.IsNullOrWhiteSpace(dept) ? null : dept,
            minSalary);

        PrintTable(list);
    }

    // ── 3 — Find by ID ───────────────────────────────────────────────────────

    private void FindById()
    {
        int id = ReadInt("ID e punonjësit");
        var e  = _service.FindById(id);

        if (e is null)
            Warn($"Punonjësi me ID {id} nuk u gjet.");
        else
        {
            Console.WriteLine();
            PrintRow(e);
        }
    }

    // ── 4 — Add ──────────────────────────────────────────────────────────────

    private void AddEmployee()
    {
        Console.WriteLine("\n  ── Shto Punonjës ──");
        string name    = Prompt("Emri dhe mbiemri");
        string pos     = Prompt("Pozita");
        string dept    = Prompt("Departamenti");
        decimal salary = ReadDecimal("Paga (€)");
        DateTime hired = ReadDate("Data e punësimit (YYYY-MM-DD)");

        var e = _service.Add(name, pos, dept, salary, hired);
        Success($"Punonjësi u shtua me ID: {e.Id}");
    }

    // ── 5 — Update ───────────────────────────────────────────────────────────

    private void UpdateEmployee()
    {
        int id = ReadInt("ID e punonjësit për ndryshim");
        var existing = _service.FindById(id)
            ?? throw new InvalidOperationException($"ID {id} nuk ekziston.");

        Console.WriteLine($"\n  Punonjësi aktual: {existing}");
        Console.WriteLine("  (Shtypni Enter për të mbajtur vlerën ekzistuese)\n");

        string name    = PromptWithDefault("Emri",         existing.Name);
        string pos     = PromptWithDefault("Pozita",       existing.Position);
        string dept    = PromptWithDefault("Departamenti", existing.Department);
        decimal salary = ReadDecimalWithDefault("Paga (€)", existing.Salary);
        DateTime hired = ReadDateWithDefault("Data e punësimit", existing.HiredAt);

        _service.Update(id, name, pos, dept, salary, hired);
        Success($"Punonjësi me ID {id} u përditësua.");
    }

    // ── 6 — Delete ───────────────────────────────────────────────────────────

    private void DeleteEmployee()
    {
        int id = ReadInt("ID e punonjësit për fshirje");
        var e  = _service.FindById(id)
            ?? throw new InvalidOperationException($"ID {id} nuk ekziston.");

        Console.Write($"\n  ⚠  Jeni i sigurt se doni të fshini '{e.Name}'? (po/jo): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();
        if (confirm != "po")
        {
            Warn("Fshirja u anulua.");
            return;
        }

        _service.Delete(id);
        Success($"Punonjësi me ID {id} u fshi.");
    }

    // ── Display helpers ───────────────────────────────────────────────────────

    private static void PrintTable(List<Employee> list)
    {
        if (list.Count == 0) { Warn("Asnjë rekord nuk u gjet."); return; }

        Console.WriteLine();
        Console.WriteLine($"  {"ID",-5} {"Emri",-22} {"Pozita",-20} {"Departamenti",-15} {"Paga",10}  {"Punësuar",-12}");
        Console.WriteLine("  " + new string('─', 88));
        foreach (var e in list) PrintRow(e);
        Console.WriteLine("  " + new string('─', 88));
        Console.WriteLine($"  Gjithsej: {list.Count} punonjës.");
    }

    private static void PrintRow(Employee e)
    {
        Console.WriteLine(
            $"  {e.Id,-5} {e.Name,-22} {e.Position,-20} {e.Department,-15} €{e.Salary,9:N2}  {e.HiredAt:yyyy-MM-dd}");
    }

    // ── Input helpers ─────────────────────────────────────────────────────────

    private static string Prompt(string label)
    {
        Console.Write($"  {label}: ");
        return Console.ReadLine() ?? string.Empty;
    }

    private static string PromptWithDefault(string label, string current)
    {
        Console.Write($"  {label} [{current}]: ");
        var input = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(input) ? current : input;
    }

    private static int ReadInt(string label)
    {
        while (true)
        {
            var raw = Prompt(label);
            if (int.TryParse(raw, out var val) && val > 0) return val;
            Warn("Ju lutem shkruani numër të plotë pozitiv.");
        }
    }

    private static decimal ReadDecimal(string label)
    {
        while (true)
        {
            var raw = Prompt(label);
            if (decimal.TryParse(raw, out var val) && val > 0) return val;
            Warn("Ju lutem shkruani shumë valide (> 0).");
        }
    }

    private static decimal ReadDecimalWithDefault(string label, decimal current)
    {
        Console.Write($"  {label} [{current}]: ");
        var raw = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(raw)) return current;
        return decimal.TryParse(raw, out var val) && val > 0 ? val : current;
    }

    private static DateTime ReadDate(string label)
    {
        while (true)
        {
            var raw = Prompt(label);
            if (DateTime.TryParse(raw, out var dt)) return dt;
            Warn("Format i pavlefshëm. Shembull: 2023-06-15");
        }
    }

    private static DateTime ReadDateWithDefault(string label, DateTime current)
    {
        Console.Write($"  {label} [{current:yyyy-MM-dd}]: ");
        var raw = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(raw)) return current;
        return DateTime.TryParse(raw, out var dt) ? dt : current;
    }

    // ── Output styles ─────────────────────────────────────────────────────────

    private static void Banner()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n  ╔══════════════════════════════════════╗");
        Console.WriteLine("  ║   WorkForce KS — Sistem Menaxhimi   ║");
        Console.WriteLine("  ║        Databaza: PostgreSQL          ║");
        Console.WriteLine("  ╚══════════════════════════════════════╝\n");
        Console.ResetColor();
    }

    private static void Success(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  ✓ {msg}");
        Console.ResetColor();
    }

    private static void Warn(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  ! {msg}");
        Console.ResetColor();
    }

    private static void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  ✗ Gabim: {msg}");
        Console.ResetColor();
    }
}
