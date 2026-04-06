using WorkForceKS.Services;
using WorkForceKS.Models;

namespace WorkForceKS.UI;

/// <summary>
/// Console-based user interface for WorkForce KS.
/// Sprint 2: added Statistics feature (option 7) and robust error handling throughout.
/// Flow: UI → Service → Repository
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
                    case "1": ShowAll();          break;
                    case "2": ShowFiltered();     break;
                    case "3": FindById();         break;
                    case "4": AddEmployee();      break;
                    case "5": UpdateEmployee();   break;
                    case "6": DeleteEmployee();   break;
                    case "7": ShowStatistics();   break;
                    case "0":
                        Console.WriteLine("\n  Mirupafshim! 👋\n");
                        return;
                    default:
                        Warn("Opsion i pavlefshëm. Provoni sërish.");
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Error($"Input i pavlefshëm: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Error(ex.Message);
            }
            catch (Exception ex)
            {
                Error($"Gabim i papritur: {ex.Message}");
            }

            Console.WriteLine("\nShtypni Enter për të vazhduar...");
            Console.ReadLine();
        }
    }

    // ── Menu ─────────────────────────────────────────────────────────────────

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("  ┌──────────────────────────────────────┐");
        Console.WriteLine("  │          WORKFORCE KS  v2.0          │");
        Console.WriteLine("  ├──────────────────────────────────────┤");
        Console.WriteLine("  │  1. Listo të gjithë punonjësit       │");
        Console.WriteLine("  │  2. Listo me filtrim                 │");
        Console.WriteLine("  │  3. Kërko sipas ID                   │");
        Console.WriteLine("  │  4. Shto punonjës                    │");
        Console.WriteLine("  │  5. Ndrysho punonjës                 │");
        Console.WriteLine("  │  6. Fshi punonjës                    │");
        Console.WriteLine("  │  7. Statistika e pagave  ★ E RE      │");
        Console.WriteLine("  │  0. Dil                              │");
        Console.WriteLine("  └──────────────────────────────────────┘");
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
        var dept      = Prompt("Departamenti (lër bosh për të gjithë)").Trim();
        var salaryTxt = Prompt("Paga minimale (lër bosh për pa limit)").Trim();

        decimal? minSalary = null;

        if (!string.IsNullOrWhiteSpace(salaryTxt))
        {
            if (!decimal.TryParse(salaryTxt, out var parsed) || parsed < 0)
            {
                Warn("Ju lutem shkruani numër valid për pagën minimale.");
                return;
            }
            minSalary = parsed;
        }

        var list = _service.List(
            string.IsNullOrWhiteSpace(dept) ? null : dept,
            minSalary);

        PrintTable(list);
    }

    // ── 3 — Find by ID ───────────────────────────────────────────────────────

    private void FindById()
    {
        var idInput = Prompt("ID e punonjësit").Trim();

        if (!int.TryParse(idInput, out var id) || id <= 0)
        {
            Warn("Ju lutem shkruani numër të plotë pozitiv për ID.");
            return;
        }

        var e = _service.FindById(id);

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

        string name   = Prompt("Emri dhe mbiemri").Trim();
        string pos    = Prompt("Pozita").Trim();
        string dept   = Prompt("Departamenti").Trim();

        var salaryTxt = Prompt("Paga (€)").Trim();
        if (!decimal.TryParse(salaryTxt, out var salary) || salary <= 0)
        {
            Warn("Ju lutem shkruani shumë valide (> 0).");
            return;
        }

        var dateTxt = Prompt("Data e punësimit (YYYY-MM-DD)").Trim();
        if (!DateTime.TryParse(dateTxt, out var hired))
        {
            Warn("Format i pavlefshëm për datë. Shembull: 2023-06-15");
            return;
        }

        var e = _service.Add(name, pos, dept, salary, hired);
        Success($"Punonjësi u shtua me sukses! ID: {e.Id}");
    }

    // ── 5 — Update ───────────────────────────────────────────────────────────

    private void UpdateEmployee()
    {
        var idTxt = Prompt("ID e punonjësit për ndryshim").Trim();
        if (!int.TryParse(idTxt, out var id) || id <= 0)
        {
            Warn("Ju lutem shkruani numër të plotë pozitiv për ID.");
            return;
        }

        var existing = _service.FindById(id);
        if (existing is null)
        {
            Warn($"Punonjësi me ID {id} nuk ekziston.");
            return;
        }

        Console.WriteLine($"\n  Punonjësi aktual: {existing}");
        Console.WriteLine("  (Shtypni Enter për të mbajtur vlerën ekzistuese)\n");

        string name = PromptWithDefault("Emri",         existing.Name);
        string pos  = PromptWithDefault("Pozita",       existing.Position);
        string dept = PromptWithDefault("Departamenti", existing.Department);

        decimal salary = existing.Salary;
        var salaryTxt  = Prompt($"Paga (€) [{existing.Salary}]").Trim();
        if (!string.IsNullOrEmpty(salaryTxt))
        {
            if (!decimal.TryParse(salaryTxt, out salary) || salary <= 0)
            {
                Warn("Shumë e pavlefshme — vlera ekzistuese u ruajt.");
                salary = existing.Salary;
            }
        }

        DateTime hired = existing.HiredAt;
        var dateTxt    = Prompt($"Data e punësimit [{existing.HiredAt:yyyy-MM-dd}]").Trim();
        if (!string.IsNullOrEmpty(dateTxt))
        {
            if (!DateTime.TryParse(dateTxt, out hired))
            {
                Warn("Format i pavlefshëm — data ekzistuese u ruajt.");
                hired = existing.HiredAt;
            }
        }

        _service.Update(id, name, pos, dept, salary, hired);
        Success($"Punonjësi me ID {id} u përditësua.");
    }

    // ── 6 — Delete ───────────────────────────────────────────────────────────

    private void DeleteEmployee()
    {
        var idTxt = Prompt("ID e punonjësit për fshirje").Trim();
        if (!int.TryParse(idTxt, out var id) || id <= 0)
        {
            Warn("Ju lutem shkruani numër të plotë pozitiv për ID.");
            return;
        }

        var e = _service.FindById(id);
        if (e is null)
        {
            Warn($"Punonjësi me ID {id} nuk ekziston.");
            return;
        }

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

    // ── 7 — Statistics (SPRINT 2 — FEATURE E RE) ─────────────────────────────

    private void ShowStatistics()
    {
        Console.WriteLine("\n  ── Statistika e Pagave ──");

        var dept = Prompt("Filtro sipas departamentit (lër bosh për të gjithë)").Trim();

        var stats = _service.GetStatistics(string.IsNullOrWhiteSpace(dept) ? null : dept);

        if (stats is null)
        {
            Warn("Nuk u gjet asnjë punonjës për filtrin e zgjedhur.");
            return;
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ╔══════════════════════════════════════════════╗");
        Console.WriteLine($"  ║  Statistika: {stats.FilterApplied,-32}║");
        Console.WriteLine("  ╠══════════════════════════════════════════════╣");
        Console.WriteLine($"  ║  Numri i punonjësve : {stats.Count,-24}║");
        Console.WriteLine($"  ║  Totali i pagave    : €{stats.TotalSalary,-23:N2}║");
        Console.WriteLine($"  ║  Paga mesatare      : €{stats.AverageSalary,-23:N2}║");
        Console.WriteLine($"  ║  Paga maksimale     : €{stats.MaxSalary,-23:N2}║");
        Console.WriteLine($"  ║  Paga minimale      : €{stats.MinSalary,-23:N2}║");
        Console.WriteLine($"  ║  Paguesi më i lartë : {stats.TopEarner,-24}║");
        Console.WriteLine("  ╚══════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    // ── Display helpers ───────────────────────────────────────────────────────

    private static void PrintTable(List<Employee> list)
    {
        if (list.Count == 0) { Warn("Asnjë rekord nuk u gjet."); return; }

        Console.WriteLine();
        Console.WriteLine($"  {"ID",-5} {"Emri",-22} {"Pozita",-20} {"Departamenti",-15} {"Paga",10}  {"Punësuar",-12}");
        Console.WriteLine("  " + new string('─', 90));
        foreach (var e in list) PrintRow(e);
        Console.WriteLine("  " + new string('─', 90));
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

    // ── Output styles ─────────────────────────────────────────────────────────

    private static void Banner()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n  ╔══════════════════════════════════════════╗");
        Console.WriteLine("  ║   WorkForce KS — Sistem Menaxhimi v2.0  ║");
        Console.WriteLine("  ║          Sprint 2 — Xhafer Ibrahimi      ║");
        Console.WriteLine("  ╚══════════════════════════════════════════╝\n");
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
