using UniRx;

namespace HearthstoneParody.Data
{
    public interface ICard
    {
        public ReactiveProperty<int> Attack { get; }
        public ReactiveProperty<int> HealthPoint { get; }
        public ReactiveProperty<int> Mana { get; }
        public void Init(int attack, int healthPoint, int mana);
    }
}