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
        private readonly ICardsDatabaseLoader _cardsDatabaseLoader;

        public GameBootStrapper(Transform deckRoot, ICardsDatabaseLoader cardsDatabaseLoader)
        {
            _cardsDatabaseLoader = cardsDatabaseLoader;
        }

        public async void Initialize()
        {
            await _cardsDatabaseLoader.LoadCardsDatabase();
        }
    }
}