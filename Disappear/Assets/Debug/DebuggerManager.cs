using UnityEngine;

public class DebuggerManager : MonoBehaviour
{
    public static DebuggerManager Instance { get; set; }
    public bool UnlimitedStamina;


    public void Init()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        UnlimitedStamina = false;
    }

    public void SetInfiniteStamina(bool stamina)
    {
        UnlimitedStamina = stamina;
    }
}