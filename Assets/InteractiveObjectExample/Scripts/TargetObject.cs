using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

public class TargetObject : MonoBehaviour
{
    [SerializeField]
    private int _numberOfItems = 6;
    [SerializeField]
    private float _duration = 1f;
    [SerializeField]
    private Ease _ease = Ease.OutBounce;

    private List<GameObject> _children = new List<GameObject>();

    private IDisposable _subscription;
    

    public void Start()
    {
        //аналог метода Update
        //Мы используем вспомогательную функцию UniRx - UpdateAsObservable
        //Выбираем из нее все фрейм когда была нажата мышка используя Where
        //Превращаем поток Unit в поток Vector3 в экранных координата используя Input.mousePosition и Select
        //После чего подписываем на получившийся поток
        //И на каждое событие пытаемся двигать объект в координаты клика
        gameObject.UpdateAsObservable()
                  .Where(x => Input.GetMouseButtonDown(0))
                  .Select(_ => Input.mousePosition)
                  .Subscribe
            (
             clickPosition =>
             {
                 var position = Camera.main.ScreenToWorldPoint(clickPosition);
                 StartSequence(position);
             });
    }

    public IObservable<Unit> Move(Vector3 targetPosition)
    {
        return Observable.Create<Unit>
            (
             observable =>
             {
                 //создаем твиннер, который двигает объект в заданые координаты
                 var tweener = transform.DOMove(targetPosition, _duration).SetEase(_ease).OnComplete(observable.OnCompleted);
                 return Disposable.Create(
                                          () =>
                                          {
                                              //при отмене или окончании подписки убиваем твиннер чтобы он остановился в том месте где сейчас
                                              tweener.Kill();
                                          });
             });
    }

    public IObservable<Unit> Spawn()
    {
        return Observable.Create<Unit>
            (
             observable =>
             {
                 //просто создаем вокруг объекта кубы
                 var step = 2 * Mathf.PI / _numberOfItems;
                 for (int i = 0; i < _numberOfItems; i++)
                 {
                     var angle = step * i;
                     var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                     cube.transform.SetParent(transform, false);
                     cube.transform.localPosition = new Vector3(
                            2 * Mathf.Cos(angle),
                            2 * Mathf.Sin(angle)
                         );
                     _children.Add(cube);
                 }
                 return Disposable.Empty;
             });
    }

    private void StartSequence(Vector3 position)
    {
        Cancel();
        _subscription = Observable.Concat(Move(position), Spawn()).Subscribe
            (
             _ =>
             {
             },
             () =>
             {
                 Debug.Log("Finished");
             });
    }

    private void Cancel()
    {
        //просто удаляем кубы если они есть
        foreach (var child in _children)
        {
            Destroy(child);
        }
        _children.Clear();
        //отменяем подписку если она есть
        if (_subscription != null)
        {
            _subscription.Dispose();
        }
    }
}