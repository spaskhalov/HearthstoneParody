using System;
using System.Linq;
using DG.Tweening;
using HearthstoneParody.Presenters;
using UniRx;
using UnityEngine;
using Zenject;

namespace HearthstoneParody.Data
{
    public class Card
    {
        private Player _owner;
        
        public readonly ReactiveProperty<int> Attack;
        public readonly ReactiveProperty<int> HealthPoint;
        public readonly ReactiveProperty<int> Mana;
        public string Title;
        public string Description;
        public Sprite Art;

        public Player Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                IsInHand = _owner.CardsInHand.ObserveCountChanged()
                    .Select(i => _owner.CardsInHand.Contains(this))
                    .ToReadOnlyReactiveProperty();
                
                IsOnTable = _owner.CardsOnTable.ObserveCountChanged()
                    .Select(i => _owner.CardsOnTable.Contains(this))
                    .ToReadOnlyReactiveProperty();
            }
        }

        public BoolReactiveProperty IsHighlighted { get; } = new BoolReactiveProperty();
        public BoolReactiveProperty IsSelectedByUser { get; } = new BoolReactiveProperty();
        public BoolReactiveProperty IsAlreadyMovedInThisRound { get; } = new BoolReactiveProperty();
        public ReadOnlyReactiveProperty<bool> IsInHand { get; private set; }
        public ReadOnlyReactiveProperty<bool> IsOnTable { get; private set; }
        public ReadOnlyReactiveProperty<bool> IsDead { get; }

        public Card(CardTemplate cardTemplate)
        {
            Attack = new ReactiveProperty<int>(cardTemplate.Attack);
            HealthPoint = new ReactiveProperty<int>(cardTemplate.HealthPoint);
            Mana = new ReactiveProperty<int>(cardTemplate.Mana);
            Title = cardTemplate.Title;
            Description = cardTemplate.Description;
            Art = cardTemplate.Art;

            IsDead = HealthPoint.Select(x => x <= 0).ToReadOnlyReactiveProperty();
           
        }
        
        public class Factory : PlaceholderFactory<CardTemplate, Card>
        {
        }
        
    }
    
    
}
