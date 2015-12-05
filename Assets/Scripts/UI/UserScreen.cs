using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace UI
{
    public class UserScreen : MonoBehaviour
    {
        public UIService UIService;

        [SerializeField]
        private Text UserNameText;

        [SerializeField]
        private Button BackButton;

        public void Awake()
        {
            Assert.IsNotNull(UserNameText);
            Assert.IsNotNull(BackButton);

           
        }

        public void Init(IUser user)
        {
            UserNameText.text = user.Name;
        }
    }
}