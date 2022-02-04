using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace HearthstoneParody.Data
{
    public class Card
    {
        public ReactiveProperty<int> Attack;
        public ReactiveProperty<int> HealthPoint;
        public ReactiveProperty<int> Mana;
        public string Title;
        public string Description;
        public Sprite Art;

        public Card(CardTemplate cardTemplate)
        {
            Attack = new ReactiveProperty<int>(cardTemplate.Attack);
            HealthPoint = new ReactiveProperty<int>(cardTemplate.HealthPoint);
            Mana = new ReactiveProperty<int>(cardTemplate.Mana);
            Title = cardTemplate.Title;
            Description = cardTemplate.Description;
            Art = cardTemplate.Art;
        }
        
        public class Factory : PlaceholderFactory<CardTemplate, Card>
        {
        }
        
    }
    
    
}
