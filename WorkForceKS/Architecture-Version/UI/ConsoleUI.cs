using Services;

namespace UI;

public class ConsoleUI
{
    private readonly EmployeeService service;

    public ConsoleUI(EmployeeService s)
    {
        service = s;
    }

    public void Run()
    {
        Console.WriteLine("1. Show Employees");
        Console.WriteLine("2. Add Employee");

        var choice = Console.ReadLine();

        if (choice == "1")
        {
            var employees = service.GetEmployees();

            foreach (var e in employees)
            {
                Console.WriteLine($"{e.Id} - {e.Name} - {e.Position}");
            }
        }
        else if (choice == "2")
        {
            Console.Write("Id: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Position: ");
            string position = Console.ReadLine();

            service.AddEmployee(id, name, position);
        }
    }
}