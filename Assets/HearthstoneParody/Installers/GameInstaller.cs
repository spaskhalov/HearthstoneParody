using HearthstoneParody.Core;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private Transform cardRoot;
    [SerializeField] private GameObject cardViewPrefab;
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameBootStrapper>().AsSingle().WithArguments(cardRoot, cardViewPrefab).NonLazy();
    }
}