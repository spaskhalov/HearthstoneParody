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
        public PlayerPresenterBase Owner { get; }
        public RectTransform RectTransform { get; }
        public void Init(Card card, PlayerPresenterBase root);

        public event Action<ICardPresenter, PointerEventData> IsDraggedEvent;
        public event Action<ICardPresenter, PointerEventData> PointerUpEvent;
        public event Action<ICardPresenter, PointerEventData> PointerDownEvent;
    }
}