namespace Models;

public class Employee
{
    private int id;
    private string name;
    private string position;

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Position { get => position; set => position = value; }
}