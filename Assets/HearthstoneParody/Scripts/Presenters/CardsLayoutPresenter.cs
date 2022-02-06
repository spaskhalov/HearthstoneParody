using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace HearthstoneParody.Presenters
{
    public abstract class CardsLayoutPresenter : MonoBehaviour
    {
        [SerializeField] private float animationTime = 0.5f;
        [SerializeField] private Color gizmosColor = Color.yellow;
        [SerializeField] private Transform rootForUnnecessaryCards;
        public ReactiveCollection<ICardPresenter> CardsCollection { get; } = new ReactiveCollection<ICardPresenter>();

        protected abstract List<(Vector3, Quaternion)> GetPositionsAndRotations(int count);

        private void Start()
        {
            CardsCollection.ObserveAdd().Subscribe((c) =>
            {
                c.Value.RectTransform.SetParent(transform, true);
                RepositionCards();
            });
            CardsCollection.ObserveRemove().Subscribe((c) =>
            {
                c.Value.RectTransform.SetParent(rootForUnnecessaryCards, true);
                RepositionCards();
            });
        }

        private void RepositionCards()
        {
            if(!CardsCollection.Any())
                return;
            var cardTransforms = GetPositionsAndRotations(CardsCollection.Count);
            for(int i = 0; i < CardsCollection.Count; i++)
            {
                var card = CardsCollection[i];
                var cardTransform = cardTransforms[i]; 
                card.RectTransform.DOKill();
                card.RectTransform.DOMove(cardTransform.Item1, animationTime);
                card.RectTransform.DORotateQuaternion(cardTransform.Item2, animationTime);
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