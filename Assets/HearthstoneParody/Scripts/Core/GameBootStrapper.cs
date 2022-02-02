using HearthstoneParody.Data;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace HearthstoneParody.Core
{
    [UsedImplicitly]
    public class GameBootStrapper : IInitializable
    {
        private CardsDatabase _cardsDatabase;
        private readonly ICardsDatabaseLoader _cardsDatabaseLoader;
        private readonly GameObject _cardViewPrefab;
        private readonly Transform _deckRoot;

        public GameBootStrapper(ICardsDatabaseLoader cardsDatabaseLoader, GameObject cardViewPrefab, Transform deckRoot)
        {
            _cardsDatabaseLoader = cardsDatabaseLoader;
            _cardViewPrefab = cardViewPrefab;
            _deckRoot = deckRoot;
        }

        public async void Initialize()
        {
            _cardsDatabase = await _cardsDatabaseLoader.LoadCardsDatabase();
            foreach (var cardTemplate in _cardsDatabase.CardTemplates)
            {
                var card = Object.Instantiate(_cardViewPrefab, _deckRoot);
                card.GetComponent<Image>().sprite = cardTemplate.Art;
            }
        }
    }
}