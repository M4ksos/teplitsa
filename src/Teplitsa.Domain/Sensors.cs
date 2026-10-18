namespace Teplitsa.Domain;

public abstract class Sensor
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public SensorType Type { get; private set; }
    public Position Position { get; set; } = new();
    public double CurrentValue { get; protected set; }

    protected Sensor() { }                       // для EF Core
    protected Sensor(SensorType type) => Type = type;
}

// пассивный: хранит значение, отдаёт по запросу
public class PassiveSensor : Sensor
{
    private PassiveSensor() { }
    public PassiveSensor(SensorType type) : base(type) { }

    public double Read() => CurrentValue;
    public void SetValue(double value) => CurrentValue = value;
}

// активный: сам следит за допуском и извещает об отклонении
public class ActiveSensor : Sensor
{
    public double MinAllowed { get; set; }
    public double MaxAllowed { get; set; }

    public event Action<ActiveSensor, double>? DeviationDetected;

    private ActiveSensor() { }
    public ActiveSensor(SensorType type) : base(type) { }

    public void UpdateValue(double value)
    {
        CurrentValue = value;
        if (value < MinAllowed || value > MaxAllowed)
            DeviationDetected?.Invoke(this, value);
    }
}