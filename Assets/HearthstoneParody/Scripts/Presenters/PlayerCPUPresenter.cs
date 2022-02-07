using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HearthstoneParody.Data;
using UnityEngine;

namespace HearthstoneParody.Presenters
{
    public class PlayerCPUPresenter : PlayerPresenterBase
    {
        [SerializeField] private Transform otherPlayerTableArea;
        [SerializeField] private PlayerPresenterBase otherPlayerPresenter;
        public override async void StartTurn()
        {
            base.StartTurn();
            await MoveFromHand();
            await UniTask.Delay(3000);
            await AttackUserCards();
            await AttackUserProfile();
            EndTurn();
        }

        private async Task AttackUserProfile()
        {
            var attackerCards = cardsOnTablePresenter.CardPresenters
                .Where(c => c.Card.IsHighlighted.Value).ToList();
            if (!otherPlayerPresenter.Player.CardsOnTable.Any())
            {
                foreach (var cardPresenter in attackerCards)
                {
                    await AttackUser(cardPresenter, otherPlayerPresenter).AsyncWaitForCompletion();
                }
            }
        }

        private async Task AttackUserCards()
        {
            var attackerCards = cardsOnTablePresenter.CardPresenters
                .Where(c => c.Card.IsHighlighted.Value).ToList();
            foreach (var cardPresenter in attackerCards)
            {
                var target = otherPlayerTableArea.GetComponentInChildren<ICardPresenter>();
                if (target != null)
                    await AttackCard(cardPresenter, target).AsyncWaitForCompletion();
            }
        }

        private async Task MoveFromHand()
        {
            Card cardToMoveFromHand = Player.CardsInHand.FirstOrDefault(c => c.IsHighlighted.Value);
            while (cardToMoveFromHand != null)
            {
                await UniTask.Delay(1000);
                PlaceCardOnTable(cardToMoveFromHand);
                cardToMoveFromHand = Player.CardsInHand.FirstOrDefault(c => c.IsHighlighted.Value);
            }
        }
    }
}