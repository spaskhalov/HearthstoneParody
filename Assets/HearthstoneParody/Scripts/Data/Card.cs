using System;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

namespace HearthstoneParody.Data
{
    public class Card
    {
        public readonly ReactiveProperty<int> Attack;
        public readonly ReactiveProperty<int> HealthPoint;
        public readonly ReactiveProperty<int> Mana;
        public string Title;
        public string Description;
        public Sprite Art;
        public Player Owner;

        public BoolReactiveProperty IsHighlighted { get; } = new BoolReactiveProperty();
        public BoolReactiveProperty IsSelectedByUser { get; } = new BoolReactiveProperty();
        public ReadOnlyReactiveProperty<bool> IsInHand { get; }
        public ReadOnlyReactiveProperty<bool> IsOnTable { get; }
        public ReadOnlyReactiveProperty<bool> IsDead { get; }

        public Card(CardTemplate cardTemplate, Player owner)
        {
            Attack = new ReactiveProperty<int>(cardTemplate.Attack);
            HealthPoint = new ReactiveProperty<int>(cardTemplate.HealthPoint);
            Mana = new ReactiveProperty<int>(cardTemplate.Mana);
            Title = cardTemplate.Title;
            Description = cardTemplate.Description;
            Art = cardTemplate.Art;
            Owner = owner;
            
            IsDead = HealthPoint.Select(x => x <= 0).ToReadOnlyReactiveProperty();
            IsInHand = Owner.CardsInHand.ObserveCountChanged()
                .Select(i => Owner.CardsInHand.Contains(this))
                .ToReadOnlyReactiveProperty();
            IsOnTable = Owner.CardsOnTable.ObserveCountChanged()
                .Select(i => Owner.CardsOnTable.Contains(this))
                .ToReadOnlyReactiveProperty();
        }
        
        public class Factory : PlaceholderFactory<CardTemplate, Player, Card>
        {
        }
        
    }
    
    
}
