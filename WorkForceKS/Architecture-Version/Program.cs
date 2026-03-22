using Data;
using Services;
using UI;

var repo = new FileRepository("data.csv");
var service = new EmployeeService(repo);
var ui = new ConsoleUI(service);

ui.Run();