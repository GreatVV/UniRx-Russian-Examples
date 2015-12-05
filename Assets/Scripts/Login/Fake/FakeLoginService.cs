using UniRx;

namespace Logic.Login.Fake
{
    public class FakeLoginService : ILoginService
    {
        public IObservable<IUser> Login()
        {
            return Observable.Return<IUser>(new FakeUser());
        }
    }
}