using DG.Tweening;
using HearthstoneParody.Configs;
using HearthstoneParody.Core;
using HearthstoneParody.Data;
using HearthstoneParody.Presenters;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace HearthstoneParody.GameLogic
{
    public class GameController : IInitializable
    {
        private const float AnimSpeed = 0.5f;
        private readonly ICardsDatabaseLoader _cardsDatabaseLoader;
        private readonly ICardsDeck _deck;
        private readonly IPlayerPresenter _firstPlayer;
        private readonly IPlayerPresenter _secondPlayer;
        private readonly TMP_Text _statusText;
        private readonly Button _restartButton;
        private readonly GameBalanceConfig _gameBalanceConfig;
        private int _roundNumber;

        public GameController(ICardsDatabaseLoader cardsDatabaseLoader,
            ICardsDeck deck,
            GameControllerConfig gameControllerConfig,
            GameBalanceConfig gameBalanceConfig)
        {
            _cardsDatabaseLoader = cardsDatabaseLoader;
            _deck = deck;
            _gameBalanceConfig = gameBalanceConfig;
            _statusText = gameControllerConfig.statusText;
            _firstPlayer = gameControllerConfig.firstPlayer;
            _secondPlayer = gameControllerConfig.secondPlayer;
            _restartButton = gameControllerConfig.restartButton;
        }

        public async void Initialize()
        {
            await _cardsDatabaseLoader.LoadCardsDatabase();
            
            _statusText.DOFade(0, AnimSpeed);
            InitPlayer(_firstPlayer);
            var createAnim = InitPlayer(_secondPlayer);
            createAnim.OnComplete(() => StartTurnForPlayer(_firstPlayer));
        }

        private Tween InitPlayer(IPlayerPresenter playerPresenter)
        {
            var player = new Player {Name = playerPresenter.Name};
            player.HealthPoint.Value = _gameBalanceConfig.playerStartHealthPoint;
            player.Mana.Value = _gameBalanceConfig.playerStartMana;
            
            playerPresenter.Init(player);
            playerPresenter.TurnEndedEvent += OnTurnEndedEvent;
            playerPresenter.Player.HealthPoint
                .Skip(1)
                .Where(i => i <= 0)
                .SubscribeWithState(player, (i, presenter) =>
            {
                var winner = GetOtherPlayer(playerPresenter);
                _firstPlayer.Transform.parent.gameObject.SetActive(false);
                _secondPlayer.Transform.parent.gameObject.SetActive(false);
                SetStatusWithAnim($"{winner.Player.Name} win!");
                _restartButton.enabled = true;
            });

            var createCardsSequence = DealCardsToThePlayer(playerPresenter, 
                Random.Range(_gameBalanceConfig.minCardAtStart, _gameBalanceConfig.maxCardAtStart + 1));
            return createCardsSequence;
        }

        private Sequence DealCardsToThePlayer(IPlayerPresenter playerPresenter, int cardNumber)
        {
            var createCardsSequence = DOTween.Sequence();
            createCardsSequence
                .AppendInterval(AnimSpeed)
                .AppendCallback(() =>
                    {
                        var newCard = _deck.GetNextCard();
                        playerPresenter.Player.CardsInHand.Add(newCard);
                    }
                )
                .SetLoops(cardNumber);
            return createCardsSequence;
        }

        private void OnTurnEndedEvent(IPlayerPresenter obj)
        {
            var nextPlayer = GetOtherPlayer(obj);
            var newRound = nextPlayer == _firstPlayer;
            if (newRound)
                OnNewRound();
            
            StartTurnForPlayer(nextPlayer);
        }

        private void OnNewRound()
        {
            _roundNumber++;
            UpdatePlayerForNewRound(_firstPlayer);
            UpdatePlayerForNewRound(_secondPlayer);
        }

        private void UpdatePlayerForNewRound(IPlayerPresenter playerPresenter)
        {
            playerPresenter.Player.Mana.Value = _gameBalanceConfig.playerStartMana 
                                                + _gameBalanceConfig.manaPerRoundIncrease * _roundNumber;
            
            foreach (var card in playerPresenter.Player.CardsOnTable)
                card.IsAlreadyMovedInThisRound.Value = false;
            
            DealCardsToThePlayer(playerPresenter, _gameBalanceConfig.newCardPerRound);
        }

        private IPlayerPresenter GetOtherPlayer(IPlayerPresenter obj)
        {
            return obj == _firstPlayer ? _secondPlayer : _firstPlayer;
        }

        private void StartTurnForPlayer(IPlayerPresenter player)
        {
            SetStatusWithAnim($"{player.Player.Name} turn");
            player.StartTurn();
        }

        private void SetStatusWithAnim(string text)
        {
            _statusText.text = text;
            var seq = DOTween.Sequence();
            seq.Append(_statusText.DOFade(1, AnimSpeed));
            seq.AppendInterval(AnimSpeed * 2);
            seq.Append(_statusText.DOFade(0, AnimSpeed));
        }
    }
}