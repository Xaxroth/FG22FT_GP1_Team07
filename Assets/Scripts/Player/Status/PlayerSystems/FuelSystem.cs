using UnityEngine;

public class FuelSystem 
{
    public delegate void FuelChanged();
    public event FuelChanged OnFuelChanged;

    public delegate void FuelFull();
    public event FuelFull OnFuelFull;
    
    private float _fuel;
    private float _maxFuel;
    private float _fuelConsumptionMultiplier;
    
    public float Fuel => _fuel;
    public float MaxFuel => _maxFuel;

    public FuelSystem(float fuelMax, float startingFuel, float fuelConsumptionMultiplier)
    {
        _maxFuel = fuelMax;
        _fuel = startingFuel;
        _fuelConsumptionMultiplier = fuelConsumptionMultiplier;
    }
    
    public float GetFuelPercent()
    {
        return _fuel / _maxFuel;
    }
    
    /// <summary>
    /// Consumes fuel by the given amount.
    /// </summary>
    /// <param name="amount"></param>
    public void ConsumeFuel(float amount)
    {
        if(amount <= 0) return;
        _fuel -= amount;
        _fuel = Mathf.Max(_fuel, 0);
        OnFuelChanged?.Invoke();
    }
    
    /// <summary>
    /// Consumes fuel by the given amount time the fuel consumption multiplier.
    /// </summary>
    /// <param name="amount"></param>
    public void BurnFuel(float amount)
    {
        if(amount <= 0) return;
        _fuel -= amount * _fuelConsumptionMultiplier;
        _fuel = Mathf.Max(_fuel, 0);
        OnFuelChanged?.Invoke();
    }
    
    /// <summary>
    /// Recharges the fuel by the given amount.
    /// </summary>
    /// <param name="amount"></param>
    public void RechargeFuel(float amount)
    {
        if(amount <= 0) return;
        _fuel += amount;
        if(_fuel >= _maxFuel) OnFuelFull?.Invoke();
        _fuel = Mathf.Min(_fuel, _maxFuel);
        OnFuelChanged?.Invoke();
    }

}
