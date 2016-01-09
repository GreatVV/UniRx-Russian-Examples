using DG.Tweening;
using UniRx;

public static class DoTweenObservableExtenstions
{
    public static IObservable<Unit> ToObservable(this Tweener tweener)
    {
        return Observable.Create<Unit>
            (
             observable =>
             {
                 tweener.OnComplete(observable.OnCompleted);
                 return Disposable.Empty;
             });
    }
}