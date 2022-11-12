namespace Minipede.Utility
{
    public interface IListener<T>
    {
        void Notify( T message );
    }
}
