using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HearthstoneParody.Core;
using HearthstoneParody.Data;
using HearthstoneParody.GameLogic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace HearthstoneParody.Presenters
{
    public interface IPlayerPresenter
    {
        string Name { get; }
        Player Player { get; }
        Transform Transform { get; }
        event Action<IPlayerPresenter> TurnEndedEvent;
        void StartTurn();
        void Init(Player player);
        
    }

    [RequireComponent(typeof(RectTransform))]
    public abstract class PlayerPresenterBase : MonoBehaviour, IPlayerPresenter
    {
        [SerializeField] protected float animationTime = 0.5f;
        [SerializeField] protected CardsLayoutPresenter cardsInHandPresenter;
        [SerializeField] protected CardsLayoutPresenter cardsOnTablePresenter;
        [SerializeField] protected Image boomImage;
        [SerializeField] private TMP_Text healthPointText;
        [SerializeField] private TMP_Text manaText;
        
        public event Action<IPlayerPresenter> TurnEndedEvent;
        public string Name => gameObject.name;
        public Transform Transform => transform;
        public Player Player { get; private set; }
        private ReactiveProperty<bool> IsPlayerTurn { get; } = new ReactiveProperty<bool>();
        private readonly List<ICardPresenter> _createdCardPresenters = new List<ICardPresenter>();
        private CardPresenterFactory _cardPresenterFactory;
        
        public void Init(Player player)
        {
            Player = player;
            
            cardsInHandPresenter.Init(Player.CardsInHand, CreateOrGetCardPresenter);
            cardsOnTablePresenter.Init(Player.CardsOnTable, CreateOrGetCardPresenter);

            Player.HealthPoint.SubscribeWithCounterAnim(healthPointText);
            Player.Mana.SubscribeWithCounterAnim(manaText);
        }
        
        public virtual void StartTurn()
        {
            IsPlayerTurn.Value = true;
        }

        protected virtual void EndTurn()
        {
            IsPlayerTurn.Value = false;
            TurnEndedEvent?.Invoke(this);
        }

        [Inject]
        private void InjectValues(CardPresenterFactory cardPresenterFactory)
        {
            _cardPresenterFactory = cardPresenterFactory;
        }

        protected void PlaceCardOnTable(Card card)
        {
            Player.CardsInHand.Remove(card);
            card.IsAlreadyMovedInThisRound.Value = true;
            Player.Mana.Value -= card.Mana.Value;
            Player.CardsOnTable.Add(card);
        }
        
        protected virtual void ConfigureNewCardPresenter(ICardPresenter cardPresenter)
        {
            cardPresenter.Card.IsDead.Skip(1).SubscribeWithState(
                cardPresenter, (b, cp) => KillCard(cp));

            SetupHighlight(cardPresenter.Card);
        }
        
        protected Sequence AttackCard(ICardPresenter userCard, ICardPresenter cardToAttack)
        {
            return AttackByCard(userCard, cardToAttack.RectTransform.position, cardToAttack.Card.HealthPoint);
        }
        
        protected Sequence AttackUser(ICardPresenter userCard, IPlayerPresenter tryToAttackUser)
        {
            return AttackByCard(userCard, tryToAttackUser.Transform.position, tryToAttackUser.Player.HealthPoint);
        }
        
        private Sequence AttackByCard(ICardPresenter userCard, Vector3 attackPosition, ReactiveProperty<int> targetHealth)
        {
            Player.CardsOnTable.Remove(userCard.Card);
            var sequence = DOTween.Sequence();
            boomImage.transform.position = attackPosition;
            sequence.Append(userCard.RectTransform
                .DOMove(attackPosition, animationTime)
                .SetEase(Ease.InQuad));
            sequence.AppendCallback(() =>
            {
                userCard.Card.IsAlreadyMovedInThisRound.Value = true;
                targetHealth.Value -= userCard.Card.Attack.Value;
                Player.CardsOnTable.Add(userCard.Card);
            });
            sequence.Append(boomImage.DOFade(1, animationTime / 2));
            sequence.Join(boomImage.rectTransform.DOShakeScale(animationTime, 3f));
            sequence.Append(boomImage.DOFade(0, animationTime / 2));
            return sequence;
        }

        private void KillCard(ICardPresenter cardPresenter)
        {
            Player.CardsOnTable.Remove(cardPresenter.Card);
            cardPresenter.RectTransform.DOKill();
            cardPresenter.RectTransform
                .DOMove(transform.position, animationTime)
                .OnComplete(() => Destroy(cardPresenter.RectTransform.gameObject));
        }

        private ICardPresenter CreateOrGetCardPresenter(Card c)
        {
            var alreadyCreated = _createdCardPresenters.FirstOrDefault(cp => cp.Card == c);
            if (alreadyCreated != null)
                return alreadyCreated;
            
            var presenter = _cardPresenterFactory.Create(c, this);
            ConfigureNewCardPresenter(presenter);
            _createdCardPresenters.Add(presenter);
            return presenter;
        }

        private void SetupHighlight(Card card)
        {
            var manaIsEnough = Player.Mana.Select(x => x >= card.Mana.Value);
            var canMoveFromHand = card.IsInHand.CombineByAnd(manaIsEnough);
            var canMoveFromTable = card.IsOnTable.
                StartWith(false).CombineByAnd(card.IsAlreadyMovedInThisRound.InvertValue());
            
            canMoveFromHand
                .CombineByOr(canMoveFromTable)
                .CombineByAnd(IsPlayerTurn)
                .Subscribe((c) => card.IsHighlighted.Value = c);
        }
    }
}