namespace Webgostar.Framework.Base.BaseModels
{
    public abstract class BaseDomainEvent
    {
        public long Id { get; set; }
        public long? CreateDate { get; protected set; } = DateTime.Now.Ticks;
        public long? CreateUserId { get; protected set; }
        public long? DeleteDate { get; protected set; }
        public long? DeleteUserId { get; protected set; }
    }
}