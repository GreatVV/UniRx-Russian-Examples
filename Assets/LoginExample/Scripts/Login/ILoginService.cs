using UniRx;

namespace Logic.Login
{
    public interface ILoginService
    {
        IObservable<IUser> Login();
    }
}