using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSystem 
{
    
    public int ShieldCharges { get; private set; }
    private readonly int _maxShieldCharges;
    public float ShieldDuration { get; private set; }
    private readonly float _maxShieldDuration;
    
    public delegate void ShieldStatusChanged();
    public ShieldStatusChanged OnShieldStatusChangedChanged;

    
    public ShieldSystem(int maxShieldCharges, float maxShieldDuration, int startCharges = 0)
    {
        _maxShieldCharges = maxShieldCharges;
        _maxShieldDuration = maxShieldDuration;
        ShieldDuration = 0;
        AddShieldCharge(startCharges);
    }
    
    /// <summary>
    /// Add shield charges to the shield system
    /// Refreshes the shield duration if the shield is already active
    /// </summary>
    /// <param name="amount"></param>
    public void AddShieldCharge(int amount)
    {
        if (amount <= 0)return;
        ShieldCharges += amount;
        ShieldCharges = Mathf.Min(ShieldCharges, _maxShieldCharges);
        RefreshShieldDuration();
        OnShieldStatusChangedChanged?.Invoke();
    }

    /// <summary>
    /// Refresh shield duration to max if shield is active
    /// </summary>
    public void RefreshShieldDuration()
    {
        if(!IsShielded()) return;
        ShieldDuration = _maxShieldDuration;
        OnShieldStatusChangedChanged?.Invoke();
    }
    
    /// <summary>
    /// Remove shield charges from the shield system
    /// Refreshes the shield duration if the shield has charges left
    /// </summary>
    public void RemoveShieldCharge(int amount = 1)
    {
        if (!IsShielded()) return;
        
        ShieldCharges -= amount;
        ShieldCharges = Mathf.Max(ShieldCharges, 0);
        RefreshShieldDuration();
        OnShieldStatusChangedChanged?.Invoke();
    }
    
    /// <summary>
    /// Add shield duration to the shield system if shield is active
    /// </summary>
    /// <param name="duration"></param>
    public void AddShieldDuration(float duration)
    {
        if(!IsShielded()) return;
        
        ShieldDuration += duration;
        ShieldDuration = Mathf.Min(ShieldDuration, _maxShieldDuration);
        OnShieldStatusChangedChanged?.Invoke();
    }
    
    /// <summary>
    /// Remove shield duration from the shield system if shield is active
    /// </summary>
    /// <param name="duration"></param>
    public void RemoveShieldDuration(float duration)
    {
        if(!IsShielded()) return;
        
        ShieldDuration -= duration;
        OnShieldStatusChangedChanged?.Invoke();
        
        if (ShieldDuration > 0) return;
        
        RemoveShieldCharge();
    }
    
    /// <summary>
    /// Return true if there is at least one shield charge left
    /// </summary>
    /// <returns></returns>
    public bool IsShielded()
    {
        return ShieldCharges > 0;
    }

}
