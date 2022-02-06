using System;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace HearthstoneParody.Presenters
{
    public class PlayerPresenter : PlayerPresenterBase, IPointerClickHandler
    {
        [SerializeField] private Transform startPositionForActiveCard;
        private bool _tookCardFromTable;

        public void OnPointerClick(PointerEventData eventData)
        {
            Player.CardsInHand.Add(_deck.GetNextCard(Player));
        }

        private void OnCardPointerDownEvent(ICardPresenter cardPresenter, PointerEventData arg2)
        {
            _tookCardFromTable = cardPresenter.Card.IsOnTable.Value;
            Player.CardsOnTable.Remove(cardPresenter.Card);
            Player.CardsInHand.Remove(cardPresenter.Card);
            cardPresenter.RectTransform.SetParent(startPositionForActiveCard, true);
            cardPresenter.RectTransform.DOKill();
            cardPresenter.RectTransform.DOMove(startPositionForActiveCard.position, animationTime / 2);
            cardPresenter.RectTransform.DORotateQuaternion(startPositionForActiveCard.rotation, animationTime / 2);
        }
        
        private void OnCardPointerUpEvent(ICardPresenter userCard, PointerEventData eventData)
        {
            if (_tookCardFromTable)
                DoMoveWithCardFromTable(userCard, eventData);
            else
                DoMoveWithCardFromHand(userCard, eventData);
        }

        private void DoMoveWithCardFromHand(ICardPresenter userCard, PointerEventData eventData)
        {
            //if user drop card under table
            if (eventData.hovered.Contains(cardsOnTablePresenter.gameObject))
            {
                Player.CardsInHand.Remove(userCard.Card);
                Player.CardsOnTable.Add(userCard.Card);
            }
            else
                Player.CardsInHand.Add(userCard.Card);
        }

        private void DoMoveWithCardFromTable(ICardPresenter userCard, PointerEventData eventData)
        {
            var targetCardToAttack = eventData.hovered
                .FirstOrDefault(g => g.GetComponent<ICardPresenter>() != null)
                ?.GetComponent<ICardPresenter>();
            if (targetCardToAttack != null && targetCardToAttack.Owner != this && targetCardToAttack.Card.IsOnTable.Value)
                AttackCard(userCard, targetCardToAttack);
            else
                Player.CardsOnTable.Add(userCard.Card);
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
        
        protected override void ConfigureNewCardPresenter(ICardPresenter cardPresenter)
        {
            base.ConfigureNewCardPresenter(cardPresenter);
            cardPresenter.IsDraggedEvent += OnCardIsDraggedEvent;
            cardPresenter.PointerDownEvent += OnCardPointerDownEvent;
            cardPresenter.PointerUpEvent += OnCardPointerUpEvent;
        }
    }
}