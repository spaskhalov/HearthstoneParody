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
        [SerializeField] private float animationTime = 0.5f;
        [SerializeField] private Transform startPositionForActiveCard;
        [SerializeField] private CardsLayoutPresenter cardsInHandPresenter;
        [SerializeField] private CardsLayoutPresenter cardsOnTablePresenter;

        private readonly Player _player = new Player();
        private readonly List<ICardPresenter> _createdCardPresenters = new List<ICardPresenter>();
        private CardPresenterFactory _cardPresenterFactory;
        private ICardsDeck _deck;

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
        
        private void Start()
        {
            _player.CardsInHand.ObserveAdd().Subscribe(c => 
                cardsInHandPresenter.CardsCollection.Add(CreateOrGetCardPresenter(c.Value)));
            _player.CardsInHand.ObserveRemove().Subscribe(c =>
            {
                var targetPresenter = cardsInHandPresenter.CardsCollection.FirstOrDefault(cp => cp.Card == c.Value);
                if (targetPresenter == null) return;
                cardsInHandPresenter.CardsCollection.Remove(targetPresenter);
            });
            
            _player.CardsOnTable.ObserveAdd().Subscribe(c => 
                cardsOnTablePresenter.CardsCollection.Add(CreateOrGetCardPresenter(c.Value)));
            _player.CardsOnTable.ObserveRemove().Subscribe(c =>
            {
                var targetPresenter = cardsOnTablePresenter.CardsCollection.FirstOrDefault(cp => cp.Card == c.Value);
                if (targetPresenter == null) return;
                cardsOnTablePresenter.CardsCollection.Remove(targetPresenter);
                Destroy(targetPresenter.RectTransform.gameObject);
            });
        }

        private ICardPresenter CreateOrGetCardPresenter(Card c)
        {
            var alreadyCreated = _createdCardPresenters.FirstOrDefault(cp => cp.Card == c);
            if (alreadyCreated != null)
                return alreadyCreated;
            
            var presenter = _cardPresenterFactory.Create(c, transform);
            presenter.IsDraggedEvent += OnCardIsDraggedEvent;
            presenter.PointerDownEvent += OnCardPointerDownEvent;
            presenter.PointerUpEvent += OnCardPointerUpEvent;
            _createdCardPresenters.Add(presenter);
            return presenter;
        }

        private void OnCardPointerUpEvent(ICardPresenter cardPresenter, PointerEventData eventData)
        {
            if (eventData.hovered.Contains(cardsOnTablePresenter.gameObject))
            {
                _player.CardsInHand.Remove(cardPresenter.Card);
                _player.CardsOnTable.Add(cardPresenter.Card);
            }
            else
                cardsInHandPresenter.CardsCollection.Add(cardPresenter);
        }

        private void OnCardPointerDownEvent(ICardPresenter cardPresenter, PointerEventData arg2)
        {
            cardsInHandPresenter.CardsCollection.Remove(cardPresenter);
            cardPresenter.RectTransform.SetParent(startPositionForActiveCard, true);
            //cardPresenter.
            cardPresenter.RectTransform.DOKill();
            cardPresenter.RectTransform.DOMove(startPositionForActiveCard.position, animationTime / 2);
            cardPresenter.RectTransform.DORotateQuaternion(startPositionForActiveCard.rotation, animationTime / 2);
        }

        private void OnCardIsDraggedEvent(ICardPresenter cardPresenter, PointerEventData eventData)
        {
            var cam = eventData.pressEventCamera;
            var camPosition = cam.transform.position;
            var startPos = startPositionForActiveCard.position;
            var endPos = cardsOnTablePresenter.transform.position;
            
            var pointOnGameField = cam.ScreenToWorldPoint(
                new Vector3(eventData.position.x, eventData.position.y, endPos.z - camPosition.z));
            var totalZDelta = endPos.z - startPos.z;
            var totalYDelta = endPos.y - startPos.y;
            var currentYDelta =  pointOnGameField.y - startPos.y;
            var zDelta = currentYDelta * totalZDelta / totalYDelta;
            //clamp z between start and end pos
            var targetZ = Mathf.Clamp(startPos.z + zDelta, startPos.z, endPos.z);

            var targetPos = cam.ScreenToWorldPoint(
                new Vector3(eventData.position.x, eventData.position.y, targetZ - camPosition.z));
            cardPresenter.RectTransform.DOMove(targetPos, animationTime / 2);
        }
    }
}