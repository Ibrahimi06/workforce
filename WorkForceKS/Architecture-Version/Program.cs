using System;
using WorkForceKS.Repositories;
using WorkForceKS.Services;
using WorkForceKS.UI;
using WorkForceKS.Data;
using WorkForceKS.Models;

// Toggle: true = CSV (per detyre), false = Postgres (bonus)
bool useFile = true;

IRepository<Employee> repository;

if (useFile)
{
    repository = new FileRepository();
}
else
{
    var connectionString =
        Environment.GetEnvironmentVariable("DATABASE_URL")
        ?? "Host=localhost;Database=punonjesit_ks;Username=postgres;Password=postgres;Port=5432";

    var pgRepo = new PostgresRepository(connectionString);
    pgRepo.SeedIfEmpty();

    repository = pgRepo;
}

var service = new EmployeeService(repository);
var ui      = new ConsoleUI(service);

ui.Run();