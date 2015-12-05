using System;
using UnityEngine;
using UniRx;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI
{
    public class LoginScreen : MonoBehaviour
    {
        public LoginService LoginService;

        public UIService UIService;

        public Button LoginButton;

        public Text ErrorMessage;

        public void Awake()
        {
            Assert.IsNotNull(LoginButton);
            Assert.IsNotNull(LoginService);
            Assert.IsNotNull(UIService);
            Assert.IsNotNull(ErrorMessage);

            LoginButton.OnClickAsObservable().Subscribe(_ => OnLoginButtonClicked());
        }

        public void OnEnable()
        {
            ErrorMessage.text = String.Empty;
        }

        public void OnLoginButtonClicked()
        {
            LoginService.Login().Subscribe
                (
                 user =>
                 {
                     UIService.GoToUserScreen(user);
                 },
                 error =>
                 {
                     ErrorMessage.text = error.Message;
                 });
        }
    }
}