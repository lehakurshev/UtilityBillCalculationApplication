namespace Domain;

public static class Rates
{
    public static IReadOnlyDictionary<string, Rate> RatesDictionary { get; } = new Dictionary<string, Rate>
    {
        ["ХВС"] = new("ХВС", 35.78m, 4.85m, UnitOfMeasurement.CubicMeter),
        ["ЭЭ"] = new("ЭЭ", 4.28m, 164, UnitOfMeasurement.KilowattHour),
        ["ЭЭ день"] = new("ЭЭ день", 4.9m, null, UnitOfMeasurement.KilowattHour),
        ["ЭЭ ночь"] = new("ЭЭ ночь", 2.31m, null, UnitOfMeasurement.KilowattHour),
        ["ГВС Теплоноситель"] = new("ГВС Теплоноситель", 35.78m, 4.01m, UnitOfMeasurement.CubicMeter),
        ["ГВС Тепловая энергия"] = new("ГВС Тепловая энергия", 998.69m, 0.05349m, UnitOfMeasurement.GigaCalorie)
    };
}