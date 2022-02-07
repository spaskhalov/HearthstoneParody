using HearthstoneParody.Configs;
using HearthstoneParody.Core;
using HearthstoneParody.Data;
using HearthstoneParody.Presenters;
using UnityEngine;
using Zenject;

namespace HearthstoneParody.Installers
{
    [CreateAssetMenu(fileName = "CoreInstaller", menuName = "Installers/CoreInstaller")]
    public class CoreInstaller : ScriptableObjectInstaller<CoreInstaller>
    {
        [SerializeField] private GameObject cardViewPrefab;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CardsDatabaseLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<ProgressLogger>().AsSingle();
            Container.BindFactory<CardTemplate, Card, Card.Factory>();
            
            Container.BindInstance(cardViewPrefab);
            Container.BindFactory<Card, PlayerPresenterBase, ICardPresenter, CardPresenterFactory>().FromFactory<CustomCardPresenterFactory>();
        }
    }
}