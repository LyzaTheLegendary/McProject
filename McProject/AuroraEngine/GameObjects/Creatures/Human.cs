using AuroraEngine.Utils;

namespace AuroraEngine.GameObjects.Creatures
{
    public class Human : Creature
    {
        private byte _armor = 0;
        private byte _hunger = 20;
        private string _username;
        public Human(ArUid id, string username) : base(id)
        {
            _username = username;
        }

        public override void DealDamage(Creature creature)
        {
            throw new NotImplementedException();
        }

        public override bool IsAlive()
        {
            throw new NotImplementedException();
        }

        public override void TakeDamage(ushort damage)
        {
            throw new NotImplementedException();
        }
    }
}
