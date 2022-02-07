using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HearthstoneParody.Data;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace HearthstoneParody.Presenters
{
    [RequireComponent(typeof(RectTransform))]
    public class CardPresenter : MonoBehaviour, ICardPresenter, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private TMP_Text attackText;
        [SerializeField] private TMP_Text healthPointText;
        [SerializeField] private TMP_Text manaText;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Image artImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Material glowMaterial;
        private RectTransform _rectTransform;

        public event Action<ICardPresenter, PointerEventData> IsDraggedEvent;
        public event Action<ICardPresenter, PointerEventData> PointerUpEvent;
        public event Action<ICardPresenter, PointerEventData> PointerDownEvent;
        
        public Card Card { get; private set; }
        public PlayerPresenterBase Owner { get; private set; }

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public void Init(Card card, PlayerPresenterBase owner)
        {
            RectTransform.SetParent(owner.transform, false);
            Owner = owner;
            Card = card;
            artImage.sprite = card.Art;
            titleText.text = card.Title;
            descriptionText.text = card.Description;

            Card.Attack.SubscribeWithCounterAnim(attackText);
            Card.HealthPoint.SubscribeWithCounterAnim(healthPointText);
            Card.Mana.SubscribeWithCounterAnim(manaText);
            card.IsHighlighted.SubscribeWithState(backgroundImage,
                (g, i) => i.material = g ? glowMaterial : null);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Card.IsSelectedByUser.Value = true;
            PointerDownEvent?.Invoke(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Card.IsSelectedByUser.Value = false;
            PointerUpEvent?.Invoke(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            IsDraggedEvent?.Invoke(this, eventData);
        }
    }
    
    
}