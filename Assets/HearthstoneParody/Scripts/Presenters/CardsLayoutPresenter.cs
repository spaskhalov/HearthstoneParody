using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HearthstoneParody.Data;
using UniRx;
using UnityEngine;

namespace HearthstoneParody.Presenters
{
    [ExecuteAlways]
    public abstract class CardsLayoutPresenter : MonoBehaviour
    {
        public readonly List<ICardPresenter> CardPresenters = new List<ICardPresenter>();
        [SerializeField] private float animationTime = 0.5f;
        [SerializeField] private Color gizmosColor = Color.yellow;
        [SerializeField] private Transform rootForUnnecessaryCards;
        private ReactiveCollection<Card> _cards;

        public void Init(ReactiveCollection<Card> cardsCollection, Func<Card, ICardPresenter> createOrGetCardPresenter)
        {
            _cards = cardsCollection;
            _cards.ObserveAdd().Subscribe(c =>
            {
                var cardPresenter = createOrGetCardPresenter(c.Value);
                cardPresenter.RectTransform.SetParent(transform, true);
                CardPresenters.Add(cardPresenter);
                RepositionCards();
            });
            _cards.ObserveRemove().Subscribe(c =>
            {
                var targetPresenter = CardPresenters.FirstOrDefault(cp => cp.Card == c.Value);
                if (targetPresenter == null) return;
                targetPresenter.RectTransform.SetParent(rootForUnnecessaryCards, true);
                CardPresenters.Remove(targetPresenter);
                RepositionCards();
            });
        }
        
        protected abstract List<(Vector3, Quaternion)> GetPositionsAndRotations(int count);

        private void RepositionCards()
        {
            if(!CardPresenters.Any())
                return;
            var cardTransforms = GetPositionsAndRotations(CardPresenters.Count);
            for(int i = 0; i < CardPresenters.Count; i++)
            {
                var cardPresenter = CardPresenters[i];
                var cardTransform = cardTransforms[i]; 
                cardPresenter.RectTransform.DOKill();
                cardPresenter.RectTransform
                    .DOMove(cardTransform.Item1, animationTime)
                    .SetEase(Ease.OutQuad);
                cardPresenter.RectTransform.DORotateQuaternion(cardTransform.Item2, animationTime);
            }
        }

        private void OnValidate()
        {
            RepositionCards();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmosColor;
            var cardTransforms = GetPositionsAndRotations(9);

            foreach (var (pos, rot) in cardTransforms)
            {
                Gizmos.DrawSphere(pos, 0.3f);
                Gizmos.DrawRay(pos, rot * Vector3.up);
            }
        }
    }
}