using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ConsoleApplication
{
    public static class BillingHelper
    {
        // Универсальный метод для повторного ввода с проверкой условия
        private static T PromptForInput<T>(string prompt, Func<string, (bool, T)> validator)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                var input = Console.ReadLine();
                var (isValid, result) = validator(input);
                if (isValid)
                {
                    return result;
                }
                Console.WriteLine("Некорректный ввод, попробуйте снова.");
            }
        }

        // Метод для получения количества дней в месяце по названию
        public static int GetDaysInMonthByName()
        {
            return PromptForInput("Введите год и месяц расчета квитанции (например: 2024 Февраль):",
                input =>
                {
                    if (string.IsNullOrWhiteSpace(input))
                        return (false, 0);

                    var parts = input.Split();
                    if (parts.Length != 2)
                        return (false, 0);

                    if (!int.TryParse(parts[0], out int year))
                        return (false, 0);

                    var monthName = parts[1];
                    var dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
                    var monthIndex = Array.FindIndex(dtfi.MonthNames, m => m.Equals(monthName, StringComparison.OrdinalIgnoreCase)) + 1;

                    if (monthIndex == 0)
                        return (false, 0);

                    return (true, DateTime.DaysInMonth(year, monthIndex));
                });
        }


        private static List<(int start, int end, int number)> GetIntervals(int daysInMonth)
        {
            var intervals = new List<(int, int, int)>();

            var numberOfIntervals = PromptForInput("Введите количество промежутков в месяце, когда в доме проживало различное количество людей:",
                input => int.TryParse(input, out var n) && n > 0 ? (true, n) : (false, 0));

            for (var i = 1; i <= numberOfIntervals; i++)
            {
                var interval = PromptForInput($"Введите промежуток №{i} - начало, конец и число проживающих через пробел:",
                    input =>
                    {
                        var parts = input.Split();
                        if (parts.Length != 3)
                            return (false, (0, 0, 0));

                        if (int.TryParse(parts[0], out int start) &&
                            int.TryParse(parts[1], out int end) &&
                            int.TryParse(parts[2], out int number) &&
                            start >= 1 && end <= daysInMonth && start <= end && number >= 0)
                        {
                            return (true, (start, end, number));
                        }
                        return (false, (0, 0, 0));
                    });
                intervals.Add(interval);
            }
            return intervals;
        }

        // Метод для вычисления среднего и деталей
        public static (decimal average, string details) GetAverageNumberOfResidentsInMonthAndDetails(int daysInMonth)
        {
            var intervals = GetIntervals(daysInMonth);
            decimal totalResidents = 0;

            foreach (var (start, end, number) in intervals)
            {
                totalResidents += number * (end - start + 1);
            }

            var average = totalResidents / daysInMonth;
            var details = string.Join("; ", intervals.Select(i => $"[{i.start}-{i.end}: {i.number}]"));

            return (average, details);
        }


        private static decimal? GetMeterReading(string waterType)
        {
            return PromptForInput($"Введите показания по {waterType}, если имеется прибор учета  (или '-' для пропуска):",
                input =>
                {
                    if (input == "-")
                        return (true, null);
                    if (decimal.TryParse(input, out var reading) && reading >= 0)
                        return (true, (decimal?)reading);
                    return (false, null);
                });
        }

        public static decimal? GetColdWaterMeterReading() => GetMeterReading("ХВС");
        public static decimal? GetHotWaterMeterReading() => GetMeterReading("ГВС");

        public static (decimal? day, decimal? night) GetDayAndNightConsumptionVolume()
        {
            return PromptForInput("Введите показания по ЭЭ за день и за ночь в соответствующем порядке через пробел (или '-' для пропуска):",
                input =>
                {
                    if (input.Trim() == "-")
                    {
                        return (true, (null, null));
                    }

                    var parts = input.Split();
                    if (parts.Length != 2 ||
                        (!decimal.TryParse(parts[0], out var dayConsumption)) ||
                        (!decimal.TryParse(parts[1], out var nightConsumption)))
                    {
                        return (false, (null, null));
                    } 

                    decimal? dayValue = dayConsumption;
                    decimal? nightValue = nightConsumption;
                    return (true, (dayValue, nightValue));
                });
        }
    }
}
