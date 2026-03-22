# UML Class Diagram

## Employee
- id: int
- name: string
- position: string
+ Id
+ Name
+ Position

## IRepository<T>
+ GetAll()
+ GetById(id)
+ Add(entity)
+ Save()

## FileRepository
- employees: List<Employee>
- filePath: string
+ GetAll()
+ GetById()
+ Add()
+ Save()

## EmployeeService
- repo: IRepository<Employee>
+ GetEmployees()
+ AddEmployee()

## ConsoleUI
- service: EmployeeService
+ Run()

## Relationships
- EmployeeService -> IRepository
- FileRepository implements IRepository
- ConsoleUI -> EmployeeService