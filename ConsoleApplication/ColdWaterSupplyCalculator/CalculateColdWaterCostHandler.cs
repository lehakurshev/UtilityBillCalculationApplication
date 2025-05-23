using Domain;
using Persistence;

namespace ConsoleApplication.ColdWaterSupplyCalculator;

public class CalculateColdWaterCostHandler
{
    private readonly Rate _rate = Rates.RatesDictionary["ХВС"];
    private readonly AppDbContext _dbContext;

    public CalculateColdWaterCostHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void Handle(UserData userData)
    {
        if (userData.ColdWaterMeterValue == null)
        {
            userData.ColdWaterCost = userData.AverageMonthlyResidents * _rate.Standard * _rate.Value;
            userData.ColdWaterSupplyIsCalculatedWithStandardVolume = true;
            return;
        }
        
        var lastColdWaterMeterValue = _dbContext.Data
            .Where(data => data.ColdWaterMeterValue != null)
            .OrderByDescending(data => data.DateСreation)
            .Select(data => data.ColdWaterMeterValue)
            .FirstOrDefault() ?? 0;

        userData.ColdWaterCost = (userData.ColdWaterMeterValue - lastColdWaterMeterValue) * _rate.Value;
    }
}