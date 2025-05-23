namespace Domain;

public class Rate
{
    public Rate(string name, decimal value, decimal? standard, UnitOfMeasurement unitOfMeasurement)
    {
        Name = name;
        Value = value;
        Standard = standard;
        StandardUnitOfMeasurement = unitOfMeasurement;
    }
    public string Name { get; set; }
    public decimal Value { get; set; }
    public Currency Currency { get; set; } // пока есть только RUB
    public decimal? Standard { get; set; }
    public UnitOfMeasurement StandardUnitOfMeasurement { get; set; }
}