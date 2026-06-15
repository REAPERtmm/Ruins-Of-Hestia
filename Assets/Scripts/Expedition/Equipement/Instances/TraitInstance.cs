using System;

[Serializable] 
public class TraitInstance
{
    public TraitRarity Rarity;

    public TraitType Type;

    public StatName Stat;

    public GemsCost Cost;

    public float Value; 

    private bool _isLocked;

    public bool IsLocked
    {
        get => _isLocked;
        set
        {
            if (_isLocked == value)
                return;

            _isLocked = value;

            OnLockChanged?.Invoke(this);
        }
    }

    public event Action<TraitInstance> OnLockChanged;
}