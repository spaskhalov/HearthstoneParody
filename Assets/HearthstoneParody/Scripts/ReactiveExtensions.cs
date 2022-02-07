using System;
using UniRx;

namespace HearthstoneParody
{
    public static class ReactiveExtensions
    {
        public static IObservable<bool> CombineByAnd(this IObservable<bool> o1, IObservable<bool> o2)
        {
            return o1.CombineLatest(o2, (b, b1) => b && b1);
        }
        
        public static IObservable<bool> CombineByOr(this IObservable<bool> o1, IObservable<bool> o2)
        {
            return o1.CombineLatest(o2, (b, b1) => b || b1);
        }

        public static IObservable<bool> InvertValue(this IObservable<bool> observable)
        {
            return observable.Select(v => !v);
        }
    }
}