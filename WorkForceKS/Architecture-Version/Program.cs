using WorkForceKS.Services;
using WorkForceKS.UI;
using WorkForceKS.Data;

var connectionString = "Host=localhost;Database=workforce;Username=postgres;Password=1234";

IRepository repository = new PostgresRepository(connectionString);
var service = new EmployeeService(repository);
var ui = new ConsoleUI(service);

ui.Run();