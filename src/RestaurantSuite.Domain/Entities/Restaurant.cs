namespace RestaurantSuite.Domain.Entities;

/// <summary>
/// Represents the single restaurant configuration entity (singleton pattern).
/// This is a configuration entity for the single-restaurant architecture.
/// </summary>
public class Restaurant
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public string Timezone { get; private set; }
    public string Currency { get; private set; }
    public string SettingsJson { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Restaurant() { }

    public static Restaurant Create(string name, string address, string timezone, string currency)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (name.Length < 3 || name.Length > 200)
            throw new ArgumentException("Name must be between 3 and 200 characters", nameof(name));

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty", nameof(address));

        if (string.IsNullOrWhiteSpace(timezone))
            throw new ArgumentException("Timezone cannot be empty", nameof(timezone));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        return new Restaurant
        {
            Id = Guid.NewGuid(),
            Name = name,
            Address = address,
            Timezone = timezone,
            Currency = currency,
            SettingsJson = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string name, string address, string timezone, string currency)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (name.Length < 3 || name.Length > 200)
            throw new ArgumentException("Name must be between 3 and 200 characters", nameof(name));

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty", nameof(address));

        if (string.IsNullOrWhiteSpace(timezone))
            throw new ArgumentException("Timezone cannot be empty", nameof(timezone));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        Name = name;
        Address = address;
        Timezone = timezone;
        Currency = currency;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSettings(string settingsJson)
    {
        if (string.IsNullOrWhiteSpace(settingsJson))
            throw new ArgumentException("Settings JSON cannot be empty", nameof(settingsJson));

        SettingsJson = settingsJson;
        UpdatedAt = DateTime.UtcNow;
    }
}
