using HearthstoneParody.Core;
using HearthstoneParody.Data;
using HearthstoneParody.GameLogic;
using HearthstoneParody.Presenters;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace HearthstoneParody.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Transform cardRoot;
        [SerializeField] private GameObject cardViewPrefab;
        [SerializeField] private Button endTurnButton;
        [SerializeField] private PlayerPresenterBase CPUPlayer;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameBootStrapper>().AsSingle().WithArguments(cardRoot).NonLazy();
            Container.BindInterfacesAndSelfTo<RandomCardsDeck>().AsSingle();
            Container.BindInstance(cardViewPrefab);
            Container.BindFactory<Card, PlayerPresenterBase, ICardPresenter, CardPresenterFactory>().FromFactory<CustomCardPresenterFactory>();

            Container.BindInstance(CPUPlayer.Player).AsSingle();
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle().WithArguments(endTurnButton).NonLazy();
            
        }
    }
}