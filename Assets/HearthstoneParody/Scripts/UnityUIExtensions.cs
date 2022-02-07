using System;
using DG.Tweening;
using TMPro;
using UniRx;

namespace HearthstoneParody
{
    public static class UnityUIExtensions
    {
        public static IDisposable SubscribeToText(this IObservable<string> source, TMP_Text text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x);
        }
        
        public static IDisposable SubscribeToText<T>(this IObservable<T> source, TMP_Text text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
        }
        
        public static IDisposable SubscribeWithCounterAnim(this ReactiveProperty<int> property, TMP_Text text, 
            float tickDuration = 0.1f, float shakeStrength = 0.8f)
        {
            //manual set initial value, to prevent flickering at start
            text.text = property.Value.ToString();
            return property.SubscribeWithState(text, (val, tmpText) =>
            {
                var curValue = int.Parse(tmpText.text);
                var div = val - curValue;
                if(div == 0)
                    return;
                var step = div < 0 ? -1 : 1;
                int stepsCount = Math.Abs(div);
                tmpText.rectTransform
                    .DOShakeScale(tickDuration, shakeStrength)
                    .OnStepComplete(() => text.text = (curValue += step).ToString())
                    .SetLoops(stepsCount);
            });
        }
    }
}