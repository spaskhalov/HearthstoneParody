using System;
using System.Collections.Generic;
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
    public class CardPresenter : MonoBehaviour, ICardPresenter, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
    {
        public BoolReactiveProperty IsHighlighted { get; } = new BoolReactiveProperty();
        public BoolReactiveProperty IsSelectedByUser { get; } = new BoolReactiveProperty();
        public event Action<ICardPresenter, PointerEventData> IsDraggedEvent;
        public event Action<ICardPresenter, PointerEventData> PointerUpEvent;
        public event Action<ICardPresenter, PointerEventData> PointerDownEvent;

        [SerializeField] private TMP_Text attackText;
        [SerializeField] private TMP_Text healthPointText;
        [SerializeField] private TMP_Text manaText;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Image artImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Material glowMaterial;

        private RectTransform _rectTransform;
        
        public void Init(Card card, Transform root)
        {
            RectTransform.SetParent(root, false);
            Card = card;
            artImage.sprite = card.Art;
            titleText.text = card.Title;
            descriptionText.text = card.Description;
            
            SubscribeWithCounterAnim(Card.Attack, attackText);
            SubscribeWithCounterAnim(Card.HealthPoint, healthPointText);
            SubscribeWithCounterAnim(Card.Mana, manaText);
            IsHighlighted.SubscribeWithState(backgroundImage,
                (g, i) => i.material = g ? glowMaterial : null);
            
        }

        public Card Card { get; private set; }

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsSelectedByUser.Value = true;
            PointerDownEvent?.Invoke(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsSelectedByUser.Value = false;
            PointerUpEvent?.Invoke(this, eventData);
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log($"OnDrop from card {Card.Title}");
        }

        public void OnDrag(PointerEventData eventData)
        {
            IsDraggedEvent?.Invoke(this, eventData);
        }
        
        private static IDisposable SubscribeWithCounterAnim(ReactiveProperty<int> property, TMP_Text text, 
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