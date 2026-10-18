namespace Teplitsa.Domain;

public abstract class Actuator
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public ActuatorType Type { get; private set; }
    public Position Position { get; set; } = new();
    public bool IsOn { get; private set; }

    protected Actuator() { }
    protected Actuator(ActuatorType type) => Type = type;

    public void TurnOn() => IsOn = true;
    public void TurnOff() => IsOn = false;
}

public class Heater : Actuator { public Heater() : base(ActuatorType.Heater) { } }
public class Cooler : Actuator { public Cooler() : base(ActuatorType.Cooler) { } }
public class Humidifier : Actuator { public Humidifier() : base(ActuatorType.Humidifier) { } }
public class FertilizerDispenser : Actuator { public FertilizerDispenser() : base(ActuatorType.FertilizerDispenser) { } }
public class LightSource : Actuator { public LightSource() : base(ActuatorType.LightSource) { } }