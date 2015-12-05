using System;
using System.Collections.Generic;
using Facebook.Unity;
using UniRx;
using UnityEngine;

namespace Logic.Login.Facebook
{
    public class FacebookLoginService : MonoBehaviour, ILoginService
    {
        public void Start()
        {
            FB.Init();
        }

        public IObservable<IUser> Login()
        {
            if (!FB.IsInitialized)
            {
                return Observable.Throw<IUser>(new InvalidOperationException("Facebook SDK is not initalized"));
            }

            return Observable.Create<IUser>
                (
                 observer =>
                 {
                     FacebookDelegate<ILoginResult> login = result =>
                                                            {
                                                                if (!string.IsNullOrEmpty(result.Error))
                                                                {
                                                                    observer.OnError(new Exception(result.Error));
                                                                    ;
                                                                }
                                                                else
                                                                {
                                                                    API("me?fields=name").Subscribe
                                                                        (
                                                                         graphResult =>
                                                                         {
                                                                             var facebookUser = new FacebookUser();
                                                                             facebookUser.Name =
                                                                                 graphResult.ResultDictionary["name"] as
                                                                                 string;

                                                                             observer.OnNext(facebookUser);
                                                                             observer.OnCompleted();
                                                                         },
                                                                         e =>
                                                                         {
                                                                             observer.OnError(e);
                                                                         });
                                                                }
                                                            };

                     FB.LogInWithReadPermissions
                         (
                          new List<string>()
                          {
                              "public_profile"
                          },
                          login);

                     return Disposable.Empty;
                 });
        }

        public IObservable<IGraphResult> API(string query)
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