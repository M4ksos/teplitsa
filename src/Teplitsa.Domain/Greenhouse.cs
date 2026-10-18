namespace Teplitsa.Domain;

public class Greenhouse
{
    public int Id { get; private set; }
    public string Name { get; set; } = "";
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 400;
    public List<Sensor> Sensors { get; private set; } = new();
    public List<Actuator> Actuators { get; private set; } = new();
}