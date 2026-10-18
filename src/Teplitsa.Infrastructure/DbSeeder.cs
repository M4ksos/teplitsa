using Teplitsa.Domain;

namespace Teplitsa.Infrastructure;

public static class DbSeeder
{
    public static void Seed(GreenhouseDbContext db)
    {
        if (db.GrowthPlans.Any()) return;   // уже засеяно

        // ДЕМО-данные: расписание из примера задания (15-е сутки) растянуто на весь цикл.
        // Агроном потом отредактирует его через интерфейс.
        var plan = new GrowthPlan
        {
            CropName = "Тюльпаны",
            Stages = new()
            {
                new GrowthStage
                {
                    DayFrom = 1, DayTo = 30,
                    MinPh = 5.0, MaxPh = 6.0,
                    MinHumidity = 85, MaxHumidity = 95,
                    TimeSlots = new()
                    {
                        new TimeSlot { Start = TimeSpan.Zero,           End = TimeSpan.FromHours(14), TargetTemperature = 28, LightOn = true },
                        new TimeSlot { Start = TimeSpan.FromHours(14),  End = TimeSpan.FromHours(16), TargetTemperature = 28, LightOn = false },
                        new TimeSlot { Start = TimeSpan.FromHours(16),  End = TimeSpan.FromHours(24), TargetTemperature = 22, LightOn = false },
                    }
                }
            }
        };

        var gh = new Greenhouse { Name = "Теплица №1", Width = 800, Height = 400 };

        foreach (var (x, y) in new[] { (30, 30), (725, 30), (370, 200), (25, 358), (725, 358) })
            gh.Sensors.Add(new ActiveSensor(SensorType.AirTemperature)
                { MinAllowed = 26, MaxAllowed = 30, Position = new Position { X = x, Y = y } });

        foreach (var (x, y) in new[] { (45, 218), (735, 173) })
            gh.Sensors.Add(new ActiveSensor(SensorType.AirHumidity)
                { MinAllowed = 85, MaxAllowed = 95, Position = new Position { X = x, Y = y } });

        foreach (var (x, y) in new[] { (205, 100), (525, 100), (205, 278), (525, 293) })
            gh.Sensors.Add(new ActiveSensor(SensorType.PH)
                { MinAllowed = 5.0, MaxAllowed = 6.0, Position = new Position { X = x, Y = y } });

        // один пассивный датчик, чтобы в системе были оба вида
        gh.Sensors.Add(new PassiveSensor(SensorType.Light) { Position = new Position { X = 400, Y = 100 } });

        foreach (var (x, y) in new[] { (205, 28), (545, 28), (215, 358), (555, 358) })
            gh.Actuators.Add(new Heater { Position = new Position { X = x, Y = y } });

        foreach (var (x, y) in new[] { (30, 118), (750, 273) })
            gh.Actuators.Add(new Cooler { Position = new Position { X = x, Y = y } });

        foreach (var (x, y) in new[] { (40, 283), (730, 83) })
            gh.Actuators.Add(new Humidifier { Position = new Position { X = x, Y = y } });

        foreach (var (x, y) in new[] { (135, 138), (615, 138), (130, 263), (610, 263) })
            gh.Actuators.Add(new FertilizerDispenser { Position = new Position { X = x, Y = y } });

        foreach (var (x, y) in new[] { (115, 33), (370, 33), (650, 33), (215, 193), (535, 193), (120, 358), (375, 358), (670, 358) })
            gh.Actuators.Add(new LightSource { Position = new Position { X = x, Y = y } });

        db.GrowthPlans.Add(plan);
        db.Greenhouses.Add(gh);
        db.SaveChanges();

        db.GrowthCycles.Add(new GrowthCycle
        {
            GreenhouseId = gh.Id,
            Plan = plan,
            StartedAt = DateTime.Now.AddDays(-12).AddHours(-7)   // как на рисунке: «прошло 12 суток, 7 часов»
        });
        db.SaveChanges();
    }

        public static void SeedUsers(GreenhouseDbContext db)
    {
        if (db.Users.Any()) return;

        // ДЕМО-учётки для защиты курсовой
        db.Users.AddRange(
            new User { Login = "agronom",  DisplayName = "Агроном",  Role = UserRole.Agronomist, PasswordHash = PasswordHashing.Hash("agro123") },
            new User { Login = "operator", DisplayName = "Оператор", Role = UserRole.Operator,   PasswordHash = PasswordHashing.Hash("oper123") });
        db.SaveChanges();
    }
}