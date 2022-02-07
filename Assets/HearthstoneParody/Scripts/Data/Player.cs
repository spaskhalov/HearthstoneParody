using UniRx;

namespace HearthstoneParody.Data
{
    public class Player
    {
        public Player()
        {
            CardsInHand.ObserveAdd().Subscribe((c) => c.Value.Owner = this);
            CardsOnTable.ObserveAdd().Subscribe((c) => c.Value.Owner = this);
        }
        
        public string Name;
        public readonly ReactiveProperty<int> HealthPoint = new ReactiveProperty<int>();
        public readonly ReactiveProperty<int> Mana = new ReactiveProperty<int>();
        
        public ReactiveCollection<Card> CardsInHand = new ReactiveCollection<Card>();
        public ReactiveCollection<Card> CardsOnTable = new ReactiveCollection<Card>();
    }
}