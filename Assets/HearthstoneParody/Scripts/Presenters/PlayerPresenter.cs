using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace HearthstoneParody.Presenters
{
    public class PlayerPresenter : PlayerPresenterBase
    {
        [SerializeField] private Button endTurnButton;
        [SerializeField] private Transform startPositionForActiveCard;
        private bool _tookCardFromTable;
        private Color endTurnButtonColor;
        
        public override void StartTurn()
        {
            endTurnButton.gameObject.SetActive(true);
            endTurnButton.image.DOColor(endTurnButtonColor, animationTime);
            endTurnButton.GetComponentInChildren<TMP_Text>().text = "End turn";
            base.StartTurn();
        }

        protected override void EndTurn()
        {
            endTurnButton.GetComponentInChildren<TMP_Text>().text = "Waiting...";
            endTurnButton.image.DOColor(Color.gray, animationTime);
            base.EndTurn();
        }

        protected override void ConfigureNewCardPresenter(ICardPresenter cardPresenter)
        {
            base.ConfigureNewCardPresenter(cardPresenter);
            cardPresenter.IsDraggedEvent += OnCardIsDraggedEvent;
            cardPresenter.PointerDownEvent += OnCardPointerDownEvent;
            cardPresenter.PointerUpEvent += OnCardPointerUpEvent;
        }
        
        private void Start()
        {
            endTurnButton.onClick.AddListener(EndTurn);
            endTurnButtonColor = endTurnButton.image.color;
        }

        private void OnCardPointerDownEvent(ICardPresenter cardPresenter, PointerEventData arg2)
        {
            var cardWasHighlightOnMouseDown = cardPresenter.Card.IsHighlighted.Value;
            _tookCardFromTable = cardPresenter.Card.IsOnTable.Value;
            Player.CardsOnTable.Remove(cardPresenter.Card);
            Player.CardsInHand.Remove(cardPresenter.Card);
            cardPresenter.Card.IsHighlighted.Value = cardWasHighlightOnMouseDown;
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
            if (userCard.Card.IsHighlighted.Value && eventData.hovered.Contains(cardsOnTablePresenter.gameObject))
                PlaceCardOnTable(userCard.Card);
            else
                Player.CardsInHand.Add(userCard.Card);
        }

        private void DoMoveWithCardFromTable(ICardPresenter userCard, PointerEventData eventData)
        {
            var targetCardToAttack = eventData.hovered
                .FirstOrDefault(g => g.GetComponent<ICardPresenter>() != null)
                ?.GetComponent<ICardPresenter>();
            var tryToAttackUser = eventData.hovered
                .FirstOrDefault(g => g.GetComponent<IPlayerPresenter>() != null)
                ?.GetComponent<IPlayerPresenter>();
            
            if (userCard.Card.IsHighlighted.Value && targetCardToAttack != null 
                                                  && targetCardToAttack.Owner != this 
                                                  && targetCardToAttack.Card.IsOnTable.Value)
                AttackCard(userCard, targetCardToAttack);
            else if (userCard.Card.IsHighlighted.Value && tryToAttackUser != null
                                                       && !ReferenceEquals(tryToAttackUser, this)
                                                       && !tryToAttackUser.Player.CardsOnTable.Any())
                AttackUser(userCard, tryToAttackUser);
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
    }
}