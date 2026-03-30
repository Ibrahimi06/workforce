using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WorkForceKS.Models;

namespace WorkForceKS.Repositories
{
    public class FileRepository : IRepository<Employee>
    {
        private readonly string filePath = "data/employees.csv";

        public List<Employee> GetAll()
        {
            if (!File.Exists(filePath))
                return new List<Employee>();

            var lines = File.ReadAllLines(filePath).Skip(1); // skip header

            return lines.Select(line =>
            {
                var parts = line.Split(',');

                return new Employee
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    Role = parts[2],
                    Salary = decimal.Parse(parts[3]),
                    HoursWorked = int.Parse(parts[4]),
                    Department = parts[5]
                };
            }).ToList();
        }

        public Employee GetById(int id)
        {
            return GetAll().FirstOrDefault(e => e.Id == id);
        }

        public void Add(Employee employee)
        {
            var employees = GetAll();

            employee.Id = employees.Any() ? employees.Max(e => e.Id) + 1 : 1;

            employees.Add(employee);
            Save(employees);
        }

        public void Update(Employee updated)
        {
            var employees = GetAll();

            var index = employees.FindIndex(e => e.Id == updated.Id);
            if (index != -1)
            {
                employees[index] = updated;
                Save(employees);
            }
        }

        public void Delete(int id)
        {
            var employees = GetAll();

            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp != null)
            {
                employees.Remove(emp);
                Save(employees);
            }
        }

        public void Save(List<Employee> employees)
        {
            var lines = new List<string>
            {
                "Id,Name,Role,Salary,HoursWorked,Department"
            };

            lines.AddRange(employees.Select(e =>
                $"{e.Id},{e.Name},{e.Role},{e.Salary},{e.HoursWorked},{e.Department}"
            ));

            File.WriteAllLines(filePath, lines);
        }
    }
}