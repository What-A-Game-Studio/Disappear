using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }
    [SerializeField] private GameObject notificationGO;

    [SerializeField] private float notificationDisplayedTime;
    private float elapsedTime;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void DisplayNotification(string notif)
    {
        GameObject obj = Instantiate(notificationGO, transform);
        obj.GetComponent<TextMeshProUGUI>().text = notif;
        StartCoroutine(DestroyNotification(5f, obj));
    }

    private IEnumerator DestroyNotification(float waitingTime, GameObject notifToDestroy)
    {
        yield return new WaitForSeconds(waitingTime);
        Destroy(notifToDestroy);
    }
}
