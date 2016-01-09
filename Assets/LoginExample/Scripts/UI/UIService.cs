using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI
{
    public class UIService : MonoBehaviour
    {
        public UserScreen UserScreen;
        public LoginScreen LoginScreen;

        private void Awake()
        {
            Assert.IsNotNull(UserScreen);
            Assert.IsNotNull(LoginScreen);
        }

        public void Start()
        {
            GoToLoginScreen();
        }

        public void GoToUserScreen(IUser user)
        {
            LoginScreen.gameObject.SetActive(false);
            UserScreen.gameObject.SetActive(true);
            UserScreen.Init(user);
        }

        public void GoToLoginScreen()
        {
            LoginScreen.gameObject.SetActive(true);
            UserScreen.gameObject.SetActive(false);
        }
    }
}