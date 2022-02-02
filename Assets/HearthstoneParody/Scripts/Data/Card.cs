using UniRx;
using UnityEngine;

namespace HearthstoneParody.Data
{
    public class Card : MonoBehaviour, ICard 
    {
        public ReactiveProperty<int> Attack { get; private set; }
        public ReactiveProperty<int> HealthPoint { get; private set;}
        public ReactiveProperty<int> Mana { get; private set;}

        public void Init(int attack, int healthPoint, int mana)
        {
            Attack = new ReactiveProperty<int>(attack);
            HealthPoint = new ReactiveProperty<int>(healthPoint);
            Mana = new ReactiveProperty<int>(mana);
        }
    }
}
