using Microsoft.EntityFrameworkCore;
using Teplitsa.Domain;
using Teplitsa.Infrastructure;

namespace Teplitsa.Web.Services;

public class GreenhouseSimulation : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ControlSystem _control = new();
    private readonly Random _rng = new();
    private bool _initialized;

    public Greenhouse? Greenhouse { get; private set; }
    public GrowthCycle? Cycle { get; private set; }
    public TimeSlot? CurrentSlot { get; private set; }

    public event Action? StateChanged;

    public GreenhouseSimulation(IServiceScopeFactory scopes) => _scopes = scopes;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using (var scope = _scopes.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GreenhouseDbContext>();
            var gh = await db.Greenhouses
                .Include(g => g.Sensors)
                .Include(g => g.Actuators)
                .FirstAsync(ct);
            Cycle = await db.GrowthCycles.Include(c => c.Plan).FirstAsync(c => c.GreenhouseId == gh.Id, ct);
            Greenhouse = gh;
        }

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
        while (await timer.WaitForNextTickAsync(ct))
        {
            Tick(DateTime.Now);
            StateChanged?.Invoke();
        }
    }

    private void Tick(DateTime now)
    {
        var gh = Greenhouse!;
        var cycle = Cycle!;

        var stage = cycle.Plan.GetStageForDay(cycle.CurrentDay(now));
        var slot = stage?.GetSlotAt(now.TimeOfDay);
        CurrentSlot = slot;
        if (stage is null || slot is null) return;   // цикл закончился или для этого времени нет слота

        bool heaterOn = gh.Actuators.OfType<Heater>().Any(a => a.IsOn);
        bool coolerOn = gh.Actuators.OfType<Cooler>().Any(a => a.IsOn);
        bool humidifierOn = gh.Actuators.OfType<Humidifier>().Any(a => a.IsOn);
        bool dispenserOn = gh.Actuators.OfType<FertilizerDispenser>().Any(a => a.IsOn);

        // 1. эмуляция показаний: случайное блуждание + влияние включённых устройств
        foreach (var s in gh.Sensors)
        {
            if (s.Type == SensorType.Light && s is PassiveSensor light)
            {
                light.SetValue(slot.LightOn ? 10000 : 0);
                continue;
            }

            var range = PlanRange(s.Type, stage);
            if (range is null) continue;
            var (min, max) = range.Value;

            var span = max - min;
            var lo = min - span * 0.3;   // «чуть шире допустимых значений»
            var hi = max + span * 0.3;

            var drift = s.Type switch
            {
                SensorType.AirTemperature => (heaterOn ? 0.4 : 0) - (coolerOn ? 0.4 : 0),
                SensorType.AirHumidity    => (humidifierOn ? 1.5 : 0) - (heaterOn ? 0.8 : 0),
                SensorType.PH             => dispenserOn ? 0.05 : 0,
                _ => 0
            };
            var noise = (_rng.NextDouble() - 0.5) * span * 0.15;
            var current = _initialized ? s.CurrentValue : (min + max) / 2;
            var value = Math.Clamp(current + drift + noise, lo, hi);

            if (s is ActiveSensor active)
            {
                // допустимый коридор датчика берётся из плана для текущего момента
                var (aMin, aMax) = AllowedNow(s.Type, stage, slot)!.Value;
                active.MinAllowed = aMin;
                active.MaxAllowed = aMax;
                active.UpdateValue(value);
            }
            else if (s is PassiveSensor passive)
            {
                passive.SetValue(value);
            }
        }
        _initialized = true;

        // 2. регулирование: средние показания → команды → устройства
        double Avg(SensorType t) => gh.Sensors.Where(x => x.Type == t).Average(x => x.CurrentValue);
        var cmd = _control.Regulate(
            new Readings(Avg(SensorType.AirTemperature), Avg(SensorType.AirHumidity), Avg(SensorType.PH)),
            stage, slot);

        Apply(gh.Actuators.OfType<Heater>(), cmd.HeaterOn);
        Apply(gh.Actuators.OfType<Cooler>(), cmd.CoolerOn);
        Apply(gh.Actuators.OfType<Humidifier>(), cmd.HumidifierOn);
        Apply(gh.Actuators.OfType<FertilizerDispenser>(), cmd.DispenserOn);
        Apply(gh.Actuators.OfType<LightSource>(), cmd.LightOn);
    }

    private static void Apply(IEnumerable<Actuator> devices, bool on)
    {
        foreach (var d in devices) { if (on) d.TurnOn(); else d.TurnOff(); }
    }

    // диапазон по всему этапу: от него считаем границы эмуляции
    private static (double Min, double Max)? PlanRange(SensorType t, GrowthStage st) => t switch
    {
        SensorType.AirTemperature => (st.TimeSlots.Min(x => x.TargetTemperature - x.Tolerance),
                                      st.TimeSlots.Max(x => x.TargetTemperature + x.Tolerance)),
        SensorType.AirHumidity => (st.MinHumidity, st.MaxHumidity),
        SensorType.PH => (st.MinPh, st.MaxPh),
        _ => null
    };

    // допустимый коридор прямо сейчас (для температуры зависит от слота)
    private static (double Min, double Max)? AllowedNow(SensorType t, GrowthStage st, TimeSlot slot) => t switch
    {
        SensorType.AirTemperature => (slot.TargetTemperature - slot.Tolerance, slot.TargetTemperature + slot.Tolerance),
        _ => PlanRange(t, st)
    };
}