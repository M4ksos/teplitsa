namespace Teplitsa.Domain;

public class TimeSlot
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public double TargetTemperature { get; set; }
    public double Tolerance { get; set; } = 1.0;   // допуск вокруг цели, чтобы реле не дребезжало
    public bool LightOn { get; set; }
}

public class GrowthStage
{
    public int DayFrom { get; set; }
    public int DayTo { get; set; }
    public double MinPh { get; set; }
    public double MaxPh { get; set; }
    public double MinHumidity { get; set; }
    public double MaxHumidity { get; set; }
    public List<TimeSlot> TimeSlots { get; set; } = new();

    public TimeSlot? GetSlotAt(TimeSpan timeOfDay) =>
        TimeSlots.FirstOrDefault(s => timeOfDay >= s.Start && timeOfDay < s.End);
}

public class GrowthPlan
{
    public int Id { get; private set; }
    public string CropName { get; set; } = "";
    public List<GrowthStage> Stages { get; set; } = new();

    public GrowthStage? GetStageForDay(int day) =>
        Stages.FirstOrDefault(s => day >= s.DayFrom && day <= s.DayTo);
}

public class GrowthCycle
{
    public int Id { get; private set; }
    public int GreenhouseId { get; set; }
    public GrowthPlan Plan { get; set; } = null!;
    public DateTime StartedAt { get; set; }

    public TimeSpan Elapsed(DateTime now) => now - StartedAt;

    // «прошло 12 суток, 7 часов» = идёт 13-е сутки
    public int CurrentDay(DateTime now) => (int)Elapsed(now).TotalDays + 1;
}