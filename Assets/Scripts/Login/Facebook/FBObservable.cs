using System;
using Facebook.Unity;
using UniRx;

namespace Logic.Login.Facebook
{
    public class FBObservable
    {
        public static IObservable<IGraphResult> API(string query)
        {
            return Observable.Create<IGraphResult>
                (
                 observer =>
                 {
                     FacebookDelegate<IGraphResult> callback = x =>
                                                               {
                                                                   if (string.IsNullOrEmpty(x.Error))
                                                                   {
                                                                       observer.OnNext(x);
                                                                       observer.OnCompleted();
                                                                   }
                                                                   else
                                                                   {
                                                                       observer.OnError(new Exception(x.Error));
                                                                   }
                                                               };
                     FB.API(query, HttpMethod.GET, callback);
                     return Disposable.Empty;
                 });
        }
    }
}