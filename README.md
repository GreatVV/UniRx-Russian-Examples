
# Пример 1. Интерактивный объект на карте


Постановка задачи
На сцене имеется куб. При клике в любом месте экрана запускается последовательность действий: 
Объект двигается к точке клика 
При окончании движения вокруг объекта создаются кубы.
На этом примере я покажу вам как создавать холодные очереди и оборачивать вызовы к внешним библиотекам.

Для перемещения объектов будет использоваться библиотека DoTween. (http://dotween.demigiant.com/) 
DoTween достаточно популярная и удобная библиотека для твиннинга, которая неплоха в качестве примера.

Важным для дальнейшего является знание что такое холодные и горячие потоки событий. 

Холодные потоки событий - это потоки событий, которые уникальны для каждого подписчика, обычно они создаются и начинаются только тогда когда на них подписываются. Как правило они создаются с помощью метода Observable.Create

Горячие потоки событий - это потоки событий, который создают события вне зависимости от того, если подписчики или нет. Обычно для этого используются какие-нибудь реализации класса ISubject.

Опытные программисты советуют как можно чаще использовать холодные потоки событий, а горячие избегать как огня. Существует мнение, что использование горячих потоков событий является одним из признаков плохой архитектуры (хотя конечно в некоторых случаях без них не обойтись)

В UniRx существует множество вспомогательных методов для создания потоков событий (как холодных так и горячих) специфичных для Unity, например, для корутин, но в данном случае нам нужен самый общий метод из оригинальной библиотеки - Observable.Create

В целом теория проста: Каждую асинхронную операцию необходимо оборачивать в метод Observable.Create, чтобы создать холодный поток событий который начнет пушить события при подписке на него.

Вся наша логика будет в одном классе - TargetObject

```
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
        //следующая строчка - это аналог метода Update
        //Мы используем вспомогательную функцию UniRx - UpdateAsObservable
        //Выбираем из нее все фреймы, когда была нажата мышка используя Where
        //Превращаем поток Unit в поток Vector3 в экранных координата используя Input.mousePosition и Select
        //После чего подписываемся на получившийся поток
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
                 //метод Disposabe.Create(Action) - вспомогательный метод, получающий на вход Action, который вызывается каждый раз, когда вызван метод Dispose
                 return Disposable.Create(
                                          () =>
                                          {
                                              //при отмене или окончании подписки убиваем твиннер чтобы объект остановился в том месте где сейчас
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
                 observable.OnCompleted();
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
```


Давайте рассмотрим функции в этом классе. Две из них отвечают за создание потоков событий, в третьей происходит их комбинирование и последняя отвечает за отмену всех действий.

Метод Move(Vector3 targetPosition) является оберткой над методом расширения DoMove из библиотеки DoTween, который отвечает за перемещение объекта к заданной точке. 
Он создает поток событий в котором есть только одно событие - событие его окончания. 

Метод Spawn() создает необходимые кубы. Этот метод является примером того, как синхронную операцию - а создание происходит синхронно - приходится оборачивать в IObservable для того чтобы использовать как часть комбинированного потока событий.

В методе StartSequence происходит подписка на комбинированный поток событий. Два потока событий комбинируются используя метод Concat. 

Метод Concat один из самых простых методов для комбинации. Он просто складывает два потока. После завершения потока событий, происходит подписка на следующий и его значения передаются в результирующий поток.

Получившийся поток и есть конечный поток событий, на который происходит подписка.
Важно не забыть сохранить ссылку на подписку - на реализацию интерфейса IDisposable, которую вернет метод Subscribe, для того чтобы можно было отменить её. Отмена подписки происходит в методе Cancel. Для тех, кто не знаком с теорией, то отмена подписки происходит с помощью вызова метода Dispose. 
При работе с холодными потоками событий и внешними библиотеками очень важно правильно освобождать ресурсы. Именно поэтому в методе Move лямбда функция в методе Create возвращает функцию, которая убивает твиннер при отмене подписки или завершения потока. 
Я рекомендую специально обращать внимание на такие случаи при работе с внешними библиотеками.

