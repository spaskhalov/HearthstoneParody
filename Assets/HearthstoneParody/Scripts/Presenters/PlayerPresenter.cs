using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HearthstoneParody.Core;
using HearthstoneParody.Data;
using HearthstoneParody.GameLogic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Random = UnityEngine.Random;

namespace HearthstoneParody.Presenters
{
    [RequireComponent(typeof(RectTransform)), ExecuteAlways]
    public class PlayerPresenter : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField, Range(5, 50)] private float arcRadius = 20;
        [SerializeField, Range(0,Mathf.PI / 6)] private float radiansBetweenCards = Mathf.PI / 30;
        [SerializeField] private float animationTime = 0.5f;
        [SerializeField] private Transform activeCardPosition;
        [SerializeField] private Transform playerHandRoot;
        
        private readonly Player _player = new Player();
        private CardPresenterFactory _cardPresenterFactory;
        private ICardsDeck _deck;
        private ReactiveCollection<ICardPresenter> _cardsInHandPresenter = new ReactiveCollection<ICardPresenter>();
        private ReactiveCollection<ICardPresenter> _cardsOnTablePresenter = new ReactiveCollection<ICardPresenter>();

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Input.GetKey(KeyCode.D))
            {
                var cardToRemove = _player.CardsInHand[Random.Range(0, _player.CardsInHand.Count)];
                RemoveCard(cardToRemove);
            }
            else
            {
                AddNewCardToHand();
            }
        }

        private void RemoveCard(Card card)
        {
            _player.CardsInHand.Remove(card);
        }

        private void AddNewCardToHand()
        {
            _player.CardsInHand.Add(_deck.GetNextCard());
        }

        [Inject]
        private void Init(CardPresenterFactory cardPresenterFactory, ICardsDeck deck)
        {
            _cardPresenterFactory = cardPresenterFactory;
            _deck = deck;
        }
        private Vector3 ArcPivot => playerHandRoot.position + (playerHandRoot.up * -1) * arcRadius;
        private void Start()
        {
            _cardsInHandPresenter.ObserveAdd().Subscribe((c) => RepositionCards());
            _cardsInHandPresenter.ObserveRemove().Subscribe((c) => RepositionCards());

            _player.CardsInHand.ObserveAdd().Subscribe(c =>
            {
                var presenter = _cardPresenterFactory.Create(c.Value, playerHandRoot);
                presenter.IsSelectedByUser.Skip(1).SubscribeWithState(presenter, OnNextIsSelectedByUserCard);
                _cardsInHandPresenter.Add(presenter);
            });
            _player.CardsInHand.ObserveRemove().Subscribe(c =>
            {
                var targetPresenter = _cardsInHandPresenter.FirstOrDefault(cp => cp.Card == c.Value);
                if (targetPresenter == null) return;
                _cardsInHandPresenter.Remove(targetPresenter);
            });
        }

        private void OnNextIsSelectedByUserCard(bool selected, ICardPresenter c)
        {
            if (!selected)
            {
                _cardsInHandPresenter.Add(c);
                return;
            }
            
            _cardsInHandPresenter.Remove(c);
            c.RectTransform.SetSiblingIndex(int.MaxValue);
            c.RectTransform.DOKill();
            c.RectTransform.DOMove(activeCardPosition.position, animationTime / 2);
            c.RectTransform.DORotateQuaternion(activeCardPosition.rotation, animationTime / 2);
        }

        private void RepositionCards()
        {
            if(!_cardsInHandPresenter.Any())
                return;
            var angle = Mathf.PI / 2 + radiansBetweenCards * (_cardsInHandPresenter.Count - 1) / 2;
            for(int i = 0; i < _cardsInHandPresenter.Count; i++)
            {
                var card = _cardsInHandPresenter[i];
                card.RectTransform.DOKill();
                card.RectTransform.DOMove(GetPointOnCircle(angle), animationTime);
                card.RectTransform.DORotateQuaternion(Quaternion.AngleAxis((angle - Mathf.PI / 2) * Mathf.Rad2Deg,Vector3.forward), animationTime);
                angle -= radiansBetweenCards;
            }
        }

        private Vector2 GetPointOnCircle(float angle)
        {
            var pivot = ArcPivot;
            var x = pivot.x + arcRadius * Mathf.Cos(angle);
            var y = pivot.y + arcRadius * Mathf.Sin(angle);
            
            return new Vector2(x, y);
        }

        private void OnValidate()
        {
            RepositionCards();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ArcPivot, arcRadius);
            Gizmos.color = Color.yellow;
            int pointsCount = 9;
            var angle = Mathf.PI / 2 + radiansBetweenCards * (pointsCount-1) / 2;
            for (int i = 0; i < pointsCount; i++)
            {
                Gizmos.DrawSphere(GetPointOnCircle(angle), 0.2f);
                angle -= radiansBetweenCards;
            }
        }
    }
}