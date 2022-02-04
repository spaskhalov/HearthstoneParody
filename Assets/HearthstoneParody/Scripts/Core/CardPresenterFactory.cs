using Cysharp.Threading.Tasks;
using HearthstoneParody.Data;
using HearthstoneParody.Presenters;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace HearthstoneParody.Core
{
    [UsedImplicitly]
    public class CardPresenterFactory : PlaceholderFactory<Card, Transform, ICardPresenter>
    {
        
    }

    [UsedImplicitly]
    public class CustomCardPresenterFactory : IFactory<Card, Transform, ICardPresenter>
    {
        private readonly DiContainer _container;
        private readonly GameObject _cardPrefab;

        public CustomCardPresenterFactory(ICardsDatabaseProvider cardsDatabaseProvider, DiContainer container, GameObject cardPrefab)
        {
            _container = container;
            _cardPrefab = cardPrefab;
        }

        public ICardPresenter Create(Card card, Transform root)
        {
            var cardPresenter = _container.InstantiatePrefabForComponent<ICardPresenter>(_cardPrefab);
            cardPresenter.Init(card, root);
            return cardPresenter;
        }
        
    }
}