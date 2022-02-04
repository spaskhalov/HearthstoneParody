using HearthstoneParody.Core;
using HearthstoneParody.Data;
using HearthstoneParody.GameLogic;
using HearthstoneParody.Presenters;
using UnityEngine;
using Zenject;

namespace HearthstoneParody.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Transform cardRoot;
        [SerializeField] private GameObject cardViewPrefab;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameBootStrapper>().AsSingle().WithArguments(cardRoot).NonLazy();
            Container.BindInterfacesAndSelfTo<RandomCardsDeck>().AsSingle();
            Container.BindInstance(cardViewPrefab);
            Container.BindFactory<Card, Transform, ICardPresenter, CardPresenterFactory>().FromFactory<CustomCardPresenterFactory>();
        }
    }
}