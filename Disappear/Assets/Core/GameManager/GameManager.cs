using UnityEngine;

namespace WAG.Core.GM
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            //Singleton pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(gameObject);

            SwitchCursorLockMode(CursorLockMode.None, true);
        }



        public void QuitGame()
        {
#if UNITY_WEBPLAYER
     public static string webplayerQuitURL = "http://ronan-dhersignerie.fr/";
#elif UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
        }

        public void SwitchCursorLockMode(CursorLockMode lockMode, bool visible)

        {
            Cursor.lockState = lockMode;
            Cursor.visible = visible;
        }

        
    }
}