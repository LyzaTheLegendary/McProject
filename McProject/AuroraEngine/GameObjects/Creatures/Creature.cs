using AuroraEngine.Utils;

namespace AuroraEngine.GameObjects.Creatures
{
    public abstract class Creature : AuObject
    {
        private double _x, _y, _z;
        private short _Velx, _Vely, _Velz;
        //private ushort _maxHealth;
        //private ushort _currentHealth;
        //private byte _movementSpeed;
        protected Creature(ArUid id) : base(id) {}
        protected Creature() : base() { }

        //public abstract void DoTick();
        public abstract void DealDamage(Creature creature);
        public abstract void TakeDamage(ushort damage);
        public (double x, double y, double z) GetLocation() => (_x, _y, _z);
        public void SetLocation(double x, double y, double z) { _x = x; _y = y; _z = z; }
        public abstract bool IsAlive();

    }
}
