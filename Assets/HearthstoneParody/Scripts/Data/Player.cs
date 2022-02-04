using UniRx;

namespace HearthstoneParody.Data
{
    public class Player
    {
        public string Name;
        public ReactiveCollection<Card> CardsInHand = new ReactiveCollection<Card>();
        public ReactiveCollection<Card> CardsOnTable = new ReactiveCollection<Card>();
    }
}