namespace Logic.Login.Fake
{
    public class FakeUser : IUser
    {
        public string Name
        {
            get
            {
                return "Fake User Name";
            }
        }
    }
}