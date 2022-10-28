using UnityEngine;

public class DebuggerManager : MonoBehaviour
{
    public static DebuggerManager Instance { get; set; }
    public bool UnlimitedStamina;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    public void SetInfiniteStamina(bool stamina)
    {
        UnlimitedStamina = stamina;
    }
    
    
}