using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI
{
    public class LoginScreen : MonoBehaviour
    {
        [SerializeField]
        private LoginService LoginService;

        [SerializeField]
        private UIService UIService;

        [SerializeField]
        private Button LoginButton;

        [SerializeField]
        private Text ErrorMessage;

        public void Awake()
        {
            Assert.IsNotNull(LoginButton);
            Assert.IsNotNull(LoginService);
            Assert.IsNotNull(UIService);
            Assert.IsNotNull(ErrorMessage);
        }

        public void OnEnable()
        {
            ErrorMessage.text = String.Empty;
        }

        public void OnLoginButtonClicked()
        {
        }
    }
}