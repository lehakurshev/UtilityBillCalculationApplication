using System.Globalization;
using System.Text.Json;
using ConsoleApplication.ColdWaterSupplyCalculator;
using ConsoleApplication.ElectricityCostCalculator;
using ConsoleApplication.HotWaterSupplyCalculator;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace ConsoleApplication;

class Program
{
    static void Main(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        using var context = new AppDbContext(optionsBuilder.Options);
        context.Database.EnsureCreated();

        while (true)
        {
            var userData = CollectUserData();
            CalculateAndDisplayCosts(context, userData);

            AskToSaveData(context, userData);

            if (!AskToCalculateAnotherBill())
            {
                break;
            }
        }
    }
    private static UserData CollectUserData()
    {
        var daysInMonth = BillingHelper.GetDaysInMonthByName();
        
        var (averageNumberOfResidentsInMonth, numberOfResidentsInMonthDetails) =
            BillingHelper.GetAverageNumberOfResidentsInMonthAndDetails(daysInMonth);

        var coldWaterMeterReading = BillingHelper.GetColdWaterMeterReading();
        var hotWaterMeterReading = BillingHelper.GetHotWaterMeterReading();
        var dayAndNightConsumptionVolume = BillingHelper.GetDayAndNightConsumptionVolume();

        return new UserData
        (
            averageNumberOfResidentsInMonth,
            numberOfResidentsInMonthDetails,
            coldWaterMeterReading,
            hotWaterMeterReading,
            dayAndNightConsumptionVolume.night,
            dayAndNightConsumptionVolume.day
        );
    }
    
     private static void CalculateAndDisplayCosts(AppDbContext context, UserData userData)
    {
        var calculateColdWaterSupplyQueryHandler = new CalculateColdWaterCostHandler(context);
        calculateColdWaterSupplyQueryHandler.Handle(userData);

        var calculateElectricityCostQueryHandler = new CalculateElectricityCostHandler(context);
        calculateElectricityCostQueryHandler.Handle(userData);

        var calculateHotWaterSupplyQueryHandler = new CalculateHotWaterCostHandler(context);
        calculateHotWaterSupplyQueryHandler.Handle(userData);

        DisplayCosts(userData);
    }

    private static void DisplayCosts(UserData userData)
    {
        Console.WriteLine($"Начисления за ХВС: {userData.ColdWaterCost}");
        Console.WriteLine($"Начисления за ГВС: {userData.HotWaterCost} + {userData.HotWaterEnergyCost}");
        Console.WriteLine($"Начисления за ЭЭ: {userData.ElectricityCost}");
        Console.WriteLine();

        var totalCost = userData.ColdWaterCost +
                        userData.HotWaterCost +
                        userData.HotWaterEnergyCost +
                        userData.ElectricityCost;

        Console.WriteLine($"Итого к оплате: {totalCost}");
    }

    private static bool AskToSaveData(AppDbContext context, UserData userData)
    {
        while (true)
        {
            Console.WriteLine("Введите {Y} если хотите сохранить показания и начисления " +
                              "или {N} если хотите продолжить не сохраняя");
            var confirmationString = Console.ReadLine();

            if (confirmationString == "y" || confirmationString == "Y")
            {
                context.Data.Add(userData);
                context.SaveChanges();
                return true;
            }
            if (confirmationString == "n" || confirmationString == "N")
            {
                return false;
            }
        }
    }

    private static bool AskToCalculateAnotherBill()
    {
        Console.WriteLine("Введите {Y} если хотите расчитать еще одну квитанцию " +
                          "или {N} если хотите выйти из программы");
        var globalLoopConfirmationString = Console.ReadLine();

        Console.WriteLine();
        Console.WriteLine("===================================================================");
        Console.WriteLine();

        return globalLoopConfirmationString == "y" || globalLoopConfirmationString == "Y";
    }
}

/*
2025 Февраль
1
1 28 3
6,35
7,64
89,51 61,2
N
N

*/

/*
2025 Февраль
1
1 28 3
6,35
7,64
89,51 61,2
N
Y
2025 Февраль
1
1 28 3
-
-
-
N
Y
2025 Февраль
4
1 1 3
2 2 3
3 15 2
16 28 4
-
-
-
N
N

*/
