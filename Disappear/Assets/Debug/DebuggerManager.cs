using UnityEngine;
using UnityEngine.UI;

public class DebuggerManager : MonoBehaviour
{
    public static DebuggerManager Instance { get; set; }
    public bool UnlimitedStamina;
    public float debugSpeed;
    public Toggle speedToggle;
    public void Init()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        debugSpeed = 1;
        UnlimitedStamina = false;
    }

    public void SetInfiniteStamina(bool stamina)
    {
        UnlimitedStamina = stamina;
    }

    public void SetSpeedModifier()
    {
        if(speedToggle.isOn)
            debugSpeed = 5;
        else
        {
            debugSpeed = 1;
        }
    }
}