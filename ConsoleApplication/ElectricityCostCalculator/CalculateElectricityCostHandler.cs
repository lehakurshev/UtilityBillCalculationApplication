using Domain;
using Persistence;

namespace ConsoleApplication.ElectricityCostCalculator;

public class CalculateElectricityCostHandler
{
    private readonly Rate _nightRate = Rates.RatesDictionary["ЭЭ ночь"];
    private readonly Rate _dayRate = Rates.RatesDictionary["ЭЭ день"];
    private readonly Rate _rate = Rates.RatesDictionary["ЭЭ"];
    
    private readonly AppDbContext _dbContext;

    public CalculateElectricityCostHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public void Handle(UserData userData)
    {
        var dayConsumptionVolume = userData.DayElectricityMeterValue;
        var nightConsumptionVolume = userData.NightElectricityMeterValue;
        if (dayConsumptionVolume == null || nightConsumptionVolume == null)
        {
            userData.ElectricityCost = userData.AverageMonthlyResidents * _rate.Standard * _rate.Value;
            userData.ElectricityCostIsCalculatedWithStandardVolume = true;
            return;
        }

        
        var lastDayConsumptionVolume = _dbContext.Data
            .Where(data => data.DayElectricityMeterValue != null)
            .OrderByDescending(data => data.DateСreation)
            .Select(data => data.DayElectricityMeterValue)
            .FirstOrDefault() ?? 0;
        var lastNightConsumptionVolume = _dbContext.Data
            .Where(data => data.NightElectricityMeterValue != null)
            .OrderByDescending(data => data.DateСreation)
            .Select(data => data.NightElectricityMeterValue)
            .FirstOrDefault() ?? 0;
        
        userData.ElectricityCost = ((dayConsumptionVolume - lastDayConsumptionVolume) * _dayRate.Value +
               (nightConsumptionVolume - lastNightConsumptionVolume) * _nightRate.Value);
    }
}