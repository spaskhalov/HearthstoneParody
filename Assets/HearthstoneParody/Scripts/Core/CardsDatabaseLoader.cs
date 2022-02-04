using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HearthstoneParody.Configs;
using HearthstoneParody.Data;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
using Random = UnityEngine.Random;


namespace HearthstoneParody.Core
{
    public interface ICardsDatabaseLoader
    {
        UniTask<CardsDatabase> LoadCardsDatabase();
        event Action<float> ProgressUpdatedEvent;
    }

    public interface ICardsDatabaseProvider
    {
        public CardsDatabase CardsDatabase { get; }
    }

    [UsedImplicitly]
    public class CardsDatabaseLoader : ICardsDatabaseLoader, ICardsDatabaseProvider
    {
        private readonly CardsDatabaseConfig _config;
        private int _completedTasksCounter;
        private const string NamesCacheName = "NAMES_CACHE"; 

        public CardsDatabaseLoader(CardsDatabaseConfig config)
        {
            _config = config;
        }
        
        public event Action<float> ProgressUpdatedEvent;
        
        public CardsDatabase CardsDatabase { get; private set; }

        public async UniTask<CardsDatabase> LoadCardsDatabase()
        {
            _completedTasksCounter = 0;
            var names = await GetNamesDB();

            var createCardsTasks = new List<UniTask<CardTemplate>>(_config.totalCards);
            for (int i = 0; i < _config.totalCards; i++)
            {
                createCardsTasks.Add(CreateCardTemplate(names[i]).ContinueWith(IncreaseProgress));
            }
            
            var cards = await UniTask.WhenAll(createCardsTasks);

            CardsDatabase = new CardsDatabase { CardTemplates = cards };
            return CardsDatabase;
        }

        private async UniTask<string[]> GetNamesDB()
        {
            // var generateNamesUrl = _config.generateNamesUrl + _config.totalCards;
            // string text = null;
            // try
            // {
            //     Debug.Log($"Downloading random names from {_config.generateNamesUrl}... ");
            //     using var namesRequest = await UnityWebRequest.Get(generateNamesUrl).SendWebRequest().ToUniTask();
            //     if (namesRequest.result != UnityWebRequest.Result.Success)
            //         throw new Exception($"Can't get names from {generateNamesUrl}");
            //     text = namesRequest.downloadHandler.text.Replace("_", " ");
            //     PlayerPrefs.SetString(NamesCacheName, text);
            // }
            // catch (Exception e)
            // {
            //     Debug.LogError($"Can't get random names from API.\nError:{e.Message}");
            //     Debug.Log("Try to load names from PlayerPrefs");
            //     text = PlayerPrefs.GetString(NamesCacheName, null);
            // }
            //
            // if (!String.IsNullOrEmpty(text))
            //     return JsonConvert.DeserializeObject<string[]>(text);

            Debug.Log("No names in cache. We will use dumbest name ever");
            var rez = new string[_config.totalCards];
            for (int i = 0; i < _config.totalCards; i++)
                rez[i] = $"Creature {i}";
            return rez;
        }

        private async UniTask<CardTemplate> CreateCardTemplate(string title)
        {
            var mana = Random.Range(0, _config.maxMana);
            var attack = Random.Range(0, _config.maxAttack);
            var hp = Random.Range(1, _config.maxHealth);
            return new CardTemplate()
            {
                Attack = attack,
                Mana = mana,
                HealthPoint = hp,
                Art = await GetSpriteAsync(_config.artUrl),
                Title = title,
                Description = $"This magical creature costs {mana} mana, has {hp} life, and attacks with power {attack}"
            };
        }
        
        private async UniTask<Sprite> GetSpriteAsync(string url)
        {
            using var request = UnityWebRequestTexture.GetTexture(url);
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