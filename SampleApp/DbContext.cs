namespace SampleApp;

public class Person
{
    public required string Name { get; set; }
    public DateOnly BirthDay { get; set; }
}

public class Animal
{
    public required string Name { get; set; }
}

public class DbContext
{
    public List<Person> People { get; set; } = [];
    public List<Animal> Animals { get; set; } = [];
}