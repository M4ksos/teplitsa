using Microsoft.EntityFrameworkCore;
using Teplitsa.Domain;

namespace Teplitsa.Infrastructure;

public class GreenhouseDbContext : DbContext
{
    public DbSet<Greenhouse> Greenhouses => Set<Greenhouse>();
    public DbSet<GrowthPlan> GrowthPlans => Set<GrowthPlan>();
    public DbSet<GrowthCycle> GrowthCycles => Set<GrowthCycle>();
    public DbSet<User> Users => Set<User>();
    public GreenhouseDbContext(DbContextOptions<GreenhouseDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        // наследование: все виды датчиков в одной таблице, тип хранится в колонке-дискриминаторе
        b.Entity<Sensor>()
            .HasDiscriminator<string>("SensorKind")
            .HasValue<PassiveSensor>("Passive")
            .HasValue<ActiveSensor>("Active");
        b.Entity<Sensor>().Property(s => s.Type).HasConversion<string>();
        b.Entity<Sensor>().OwnsOne(s => s.Position);

        b.Entity<Actuator>()
            .HasDiscriminator<string>("ActuatorKind")
            .HasValue<Heater>("Heater")
            .HasValue<Cooler>("Cooler")
            .HasValue<Humidifier>("Humidifier")
            .HasValue<FertilizerDispenser>("FertilizerDispenser")
            .HasValue<LightSource>("LightSource");
        b.Entity<Actuator>().Property(a => a.Type).HasConversion<string>();
        b.Entity<Actuator>().OwnsOne(a => a.Position);

        // стадии и слоты не живут сами по себе, поэтому это owned-типы внутри плана
        // стадии и слоты остаются частью плана, но получают собственный простой ключ Id
        b.Entity<GrowthPlan>().OwnsMany(p => p.Stages, stage =>
        {
            stage.WithOwner().HasForeignKey("GrowthPlanId");
            stage.Property<int>("Id");
            stage.HasKey("Id");

            stage.OwnsMany(s => s.TimeSlots, slot =>
            {
                slot.WithOwner().HasForeignKey("GrowthStageId");
                slot.Property<int>("Id");
                slot.HasKey("Id");
            });
        });
        b.Entity<GrowthCycle>().HasOne(c => c.Plan).WithMany().IsRequired();
        b.Entity<GrowthCycle>().HasOne<Greenhouse>().WithMany().HasForeignKey(c => c.GreenhouseId);
        
        b.Entity<User>().HasIndex(u => u.Login).IsUnique();
        b.Entity<User>().Property(u => u.Role).HasConversion<string>();
    }
}