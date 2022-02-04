using System;
using HearthstoneParody.Data;
using UniRx;
using UnityEngine;

namespace HearthstoneParody.Presenters
{
    public interface ICardPresenter
    {
        public Card Card { get; }
        public RectTransform RectTransform { get; }
        public void Init(Card card, Transform root);
        public BoolReactiveProperty IsHighlighted { get; }
        public BoolReactiveProperty IsSelectedByUser { get; }
        public event Action<ICardPresenter> IsDragged;
        public event Action<ICardPresenter> IsDropped;

    }
}