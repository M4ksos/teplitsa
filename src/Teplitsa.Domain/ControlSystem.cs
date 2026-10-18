namespace Teplitsa.Domain;

public record Readings(double Temperature, double Humidity, double Ph);
public record ActuatorCommands(bool HeaterOn, bool CoolerOn, bool HumidifierOn, bool DispenserOn, bool LightOn);
public class ControlSystem
{
    public ActuatorCommands Regulate(Readings r, GrowthStage stage, TimeSlot slot)
    {
        bool heater = false, cooler = false, humidifier = false, dispenser = false;

        // температура: приоритетный параметр
        if (r.Temperature < slot.TargetTemperature - slot.Tolerance) heater = true;
        else if (r.Temperature > slot.TargetTemperature + slot.Tolerance) cooler = true;

        // влажность: низкая → увлажнитель; высокая → обогреватель (если не жарко)
        if (r.Humidity < stage.MinHumidity) humidifier = true;
        else if (r.Humidity > stage.MaxHumidity && !cooler) heater = true;

        // pH: ниже нормы → дозатор удобрений
        if (r.Ph < stage.MinPh) dispenser = true;

        return new ActuatorCommands(heater, cooler, humidifier, dispenser, slot.LightOn);
    }
}