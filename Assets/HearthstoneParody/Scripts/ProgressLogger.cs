using HearthstoneParody.Core;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace HearthstoneParody
{
    [UsedImplicitly]
    public class ProgressLogger : IInitializable
    {
        private readonly ICardsDatabaseLoader _cardsDatabaseLoader;

        public ProgressLogger(ICardsDatabaseLoader cardsDatabaseLoader)
        {
            _cardsDatabaseLoader = cardsDatabaseLoader;
        }

        public void Initialize()
        {
            _cardsDatabaseLoader.ProgressUpdatedEvent += f => Debug.Log($"Total progress is {f}");
        }
    }
}