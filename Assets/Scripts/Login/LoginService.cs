using UnityEngine;
using System.Collections;
using Logic.Login;
using Logic.Login.Facebook;
using Logic.Login.Fake;
using UniRx;
using UnityEngine.Assertions;

public class LoginService : MonoBehaviour, ILoginService
{
    private ILoginService CurrentLoginService = new FakeLoginService();

    void Awake()
    {
        CurrentLoginService = new GameObject("Facebook Login Service", typeof(FacebookLoginService)).GetComponent<FacebookLoginService>();
    }

    public IObservable<IUser> Login()
    {
        return CurrentLoginService.Login();
    }
}