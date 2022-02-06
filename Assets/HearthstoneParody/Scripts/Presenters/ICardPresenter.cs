using System;
using HearthstoneParody.Data;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HearthstoneParody.Presenters
{
    public interface ICardPresenter
    {
        public Card Card { get; }
        public RectTransform RectTransform { get; }
        public void Init(Card card, Transform root);
        public BoolReactiveProperty IsHighlighted { get; }
        public BoolReactiveProperty IsSelectedByUser { get; }
        public event Action<ICardPresenter, PointerEventData> IsDraggedEvent;
        public event Action<ICardPresenter, PointerEventData> PointerUpEvent;
        public event Action<ICardPresenter, PointerEventData> PointerDownEvent;
    }
}