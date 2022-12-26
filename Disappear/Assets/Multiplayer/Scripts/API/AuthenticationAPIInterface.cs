using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace WAG.Multiplayer
{
    public class AuthenticationAPIInterface : MonoBehaviour
    {
        public static async void InitializeAndSignInAsync(string name)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(name);

            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () => {
                // do nothing
                Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

    }
}
