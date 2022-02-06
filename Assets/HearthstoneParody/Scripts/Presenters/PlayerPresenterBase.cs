using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HearthstoneParody.Core;
using HearthstoneParody.Data;
using HearthstoneParody.GameLogic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace HearthstoneParody.Presenters
{
    [RequireComponent(typeof(RectTransform))]
    public class PlayerPresenterBase : MonoBehaviour
    {
        [SerializeField] protected float animationTime = 0.5f;
        [SerializeField] protected CardsLayoutPresenter cardsInHandPresenter;
        [SerializeField] protected CardsLayoutPresenter cardsOnTablePresenter;
        [SerializeField] protected Image boomImage;
        public Player Player { get; private set; } = new Player();
        private readonly List<ICardPresenter> _createdCardPresenters = new List<ICardPresenter>();
        private CardPresenterFactory _cardPresenterFactory;
        protected ICardsDeck _deck;

        [Inject]
        private void Init(CardPresenterFactory cardPresenterFactory, ICardsDeck deck)
        {
            _cardPresenterFactory = cardPresenterFactory;
            _deck = deck;
            Player.Name = gameObject.name;
        }

        public Sequence AttackCard(ICardPresenter userCard, ICardPresenter cardToAttack)
        {
            var sequence = DOTween.Sequence();
            var position = cardToAttack.RectTransform.position;
            boomImage.transform.position = position;
            sequence.Append(userCard.RectTransform
                .DOMove(position, animationTime)
                .SetEase(Ease.InQuad));
            sequence.AppendCallback(() =>
            {
                cardToAttack.Card.HealthPoint.Value -= userCard.Card.Attack.Value;
                Player.CardsOnTable.Add(userCard.Card);
            });
            sequence.Append(boomImage.DOFade(1, animationTime / 2));
            sequence.Join(boomImage.rectTransform.DOShakeScale(animationTime, 3f));
            sequence.Append(boomImage.DOFade(0, animationTime / 2));
            return sequence;
        }

        private void Start()
        {
            cardsInHandPresenter.Init(Player.CardsInHand, CreateOrGetCardPresenter);
            cardsOnTablePresenter.Init(Player.CardsOnTable, CreateOrGetCardPresenter);
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
        
        protected virtual void  ConfigureNewCardPresenter(ICardPresenter cardPresenter)
        {
            cardPresenter.Card.IsDead.Skip(1).SubscribeWithState(
                cardPresenter, (b, cp) => KillCard(cp));
        }
    }
}