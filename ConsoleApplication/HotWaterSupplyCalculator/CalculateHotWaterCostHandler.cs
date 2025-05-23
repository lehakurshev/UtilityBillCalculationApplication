using System.Runtime.InteropServices.JavaScript;
using Domain;
using Persistence;

namespace ConsoleApplication.HotWaterSupplyCalculator;

public class CalculateHotWaterCostHandler
{
    private readonly Rate WaterRate = Rates.RatesDictionary["ГВС Теплоноситель"];
    private readonly Rate EnergyRate = Rates.RatesDictionary["ГВС Тепловая энергия"];
    
    private readonly AppDbContext DbContext;

    public CalculateHotWaterCostHandler(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public void Handle(UserData userData)
    {
        if (userData.HotWaterMeterValue == null)
        {
            userData.HotWaterCost = userData.AverageMonthlyResidents * WaterRate.Standard * WaterRate.Value;
            userData.HotWaterEnergyCost = userData.AverageMonthlyResidents * WaterRate.Standard * EnergyRate.Standard * EnergyRate.Value;
            userData.HotWaterSupplyIsCalculatedWithStandardVolume = true;
            return;
        }
        
        var lastHotWaterMeterValue = DbContext.Data
            .Where(data => data.HotWaterMeterValue != null)
            .OrderByDescending(data => data.DateСreation)
            .Select(data => data.HotWaterMeterValue)
            .FirstOrDefault() ?? 0;

        userData.HotWaterCost = (userData.HotWaterMeterValue - lastHotWaterMeterValue) * WaterRate.Value;
        userData.HotWaterEnergyCost = (userData.HotWaterMeterValue - lastHotWaterMeterValue) * EnergyRate.Standard * EnergyRate.Value;
    }
}