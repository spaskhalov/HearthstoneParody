using HearthstoneParody.Configs;
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
        
        [SerializeField] private GameControllerConfig gameControllerConfig;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RandomCardsDeck>().AsSingle();
            Container.BindInstance(gameControllerConfig);
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
            
        }
    }
}