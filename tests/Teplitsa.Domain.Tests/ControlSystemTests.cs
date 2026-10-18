using Teplitsa.Domain;
using Xunit;

public class ControlSystemTests
{
    private readonly ControlSystem _sut = new();
    private readonly GrowthStage _stage = new()
    {
        DayFrom = 1, DayTo = 30,
        MinPh = 5.5, MaxPh = 6.5,
        MinHumidity = 80, MaxHumidity = 95
    };
    private readonly TimeSlot _slot = new() { TargetTemperature = 28, LightOn = true };

    [Fact]
    public void ColdAir_HeaterOn_CoolerOff()
    {
        var c = _sut.Regulate(new Readings(25, 90, 6.0), _stage, _slot);
        Assert.True(c.HeaterOn);
        Assert.False(c.CoolerOn);
    }

    [Fact]
    public void HotAir_CoolerOn_HeaterOff()
    {
        var c = _sut.Regulate(new Readings(31, 90, 6.0), _stage, _slot);
        Assert.True(c.CoolerOn);
        Assert.False(c.HeaterOn);
    }

    [Fact]
    public void LowHumidity_HumidifierOn()
    {
        var c = _sut.Regulate(new Readings(28, 70, 6.0), _stage, _slot);
        Assert.True(c.HumidifierOn);
    }

    [Fact]
    public void HighHumidity_HeaterOn_HumidifierOff()
    {
        var c = _sut.Regulate(new Readings(28, 99, 6.0), _stage, _slot);
        Assert.True(c.HeaterOn);
        Assert.False(c.HumidifierOn);
    }

    [Fact]
    public void HighHumidityButTooHot_TemperatureWins()
    {
        var c = _sut.Regulate(new Readings(31, 99, 6.0), _stage, _slot);
        Assert.True(c.CoolerOn);
        Assert.False(c.HeaterOn);
    }

    [Fact]
    public void LowPh_DispenserOn()
    {
        var c = _sut.Regulate(new Readings(28, 90, 5.0), _stage, _slot);
        Assert.True(c.DispenserOn);
    }

    [Fact]
    public void AllInRange_OnlyLightOn()
    {
        var c = _sut.Regulate(new Readings(28, 90, 6.0), _stage, _slot);
        Assert.Equal(new ActuatorCommands(false, false, false, false, true), c);
    }

    [Fact]
    public void Light_FollowsTimeSlot()
    {
        var dark = new TimeSlot { TargetTemperature = 28, LightOn = false };
        var c = _sut.Regulate(new Readings(28, 90, 6.0), _stage, dark);
        Assert.False(c.LightOn);
    }

    [Fact]
    public void CurrentDay_After12Days7Hours_Is13()
    {
        var cycle = new GrowthCycle { StartedAt = new DateTime(2010, 2, 2) };
        var now = cycle.StartedAt.AddDays(12).AddHours(7);
        Assert.Equal(13, cycle.CurrentDay(now));
    }
}