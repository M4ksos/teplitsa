namespace Teplitsa.Domain;

public enum SensorType { AirTemperature, WaterTemperature, AirHumidity, PH, Light, NutrientConcentration }
public enum ActuatorType { Heater, Cooler, Humidifier, FertilizerDispenser, LightSource }
public enum UserRole { Agronomist, Operator }

public class Position
{
    public int X { get; set; }
    public int Y { get; set; }
}