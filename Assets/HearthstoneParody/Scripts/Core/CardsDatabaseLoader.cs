using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HearthstoneParody.Configs;
using HearthstoneParody.Data;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace HearthstoneParody.Core
{
    public interface ICardsDatabaseLoader
    {
        UniTask<CardsDatabase> LoadCardsDatabase();
        event Action<float> ProgressUpdatedEvent;
    }

    [UsedImplicitly]
    public class CardsDatabaseLoader : ICardsDatabaseLoader
    {
        private readonly CardsDatabaseConfig _config;
        private int _completedTasksCounter;

        public CardsDatabaseLoader(CardsDatabaseConfig config)
        {
            _config = config;
        }

        public async UniTask<CardsDatabase> LoadCardsDatabase()
        {
            _completedTasksCounter = 0;
            var createCardsTasks = new List<UniTask<CardTemplate>>(_config.totalCards);
            for (int i = 0; i < _config.totalCards; i++)
            {
                createCardsTasks.Add(CreateCardTemplate().ContinueWith(IncreaseProgress));
            }
            
            var cards = await UniTask.WhenAll(createCardsTasks);

            return new CardsDatabase
            {
                CardTemplates = cards
            };
        }

        public event Action<float> ProgressUpdatedEvent;
        
        private async UniTask<CardTemplate> CreateCardTemplate()
        {
            var cardTemplate = new CardTemplate()
            {
                Attack = Random.Range(0, _config.maxAttack),
                Mana = Random.Range(0, _config.maxMana),
                HealthPoint = Random.Range(1, _config.maxHealth),
                Art = await GetSpriteAsync(_config.artUrl)
            };
            return cardTemplate;
        }
        
        private async UniTask<Sprite> GetSpriteAsync(string url)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            var requestResult = await request.SendWebRequest().ToUniTask();
            if (requestResult.result != UnityWebRequest.Result.Success)
                throw new Exception($"Can't download Texture from url {url}");
            var tex = DownloadHandlerTexture.GetContent(requestResult);
            return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        private CardTemplate IncreaseProgress(CardTemplate cardTemplate)
        {
            _completedTasksCounter++;
            ProgressUpdatedEvent?.Invoke((float)_completedTasksCounter/_config.totalCards);
            return cardTemplate;
        }
    }
}