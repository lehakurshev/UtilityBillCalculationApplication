using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Domain;

public class UserData
{
    public UserData() {}
    public UserData
    (
        decimal averageMonthlyResidents,
        string monthlyResidentsDetails,
        decimal? coldWaterMeterValue,
        decimal? hotWaterMeterValue,
        decimal? nightElectricityMeterValue,
        decimal? dayElectricityMeterValue
    )
    {
        Id = Guid.NewGuid();
        AverageMonthlyResidents = averageMonthlyResidents;
        MonthlyResidentsDetails = monthlyResidentsDetails;
        ColdWaterMeterValue = coldWaterMeterValue;
        HotWaterMeterValue = hotWaterMeterValue;
        NightElectricityMeterValue = nightElectricityMeterValue;
        DayElectricityMeterValue = dayElectricityMeterValue;
        DateСreation = DateTime.Now;
    }
    [Key]
    public Guid Id { get; set; }
    public decimal? AverageMonthlyResidents { get; set; }
    public string? MonthlyResidentsDetails { get; set; }
    public decimal? ColdWaterMeterValue { get; set; }
    public decimal? HotWaterMeterValue { get; set; }
    public decimal? DayElectricityMeterValue { get; set; }
    public decimal? NightElectricityMeterValue { get; set; }
    public DateTime DateСreation { get; set; }
    
    public decimal? ColdWaterCost { get; set; }
    public decimal? ElectricityCost { get; set; }
    public decimal? HotWaterCost { get; set; }
    public decimal? HotWaterEnergyCost { get; set; }
    
    public decimal? TotalCost => ColdWaterCost + ElectricityCost + HotWaterCost + HotWaterEnergyCost;

    public bool ColdWaterSupplyIsCalculatedWithStandardVolume { get; set; }
    public bool ElectricityCostIsCalculatedWithStandardVolume { get; set; }
    public bool HotWaterSupplyIsCalculatedWithStandardVolume { get; set; }
}