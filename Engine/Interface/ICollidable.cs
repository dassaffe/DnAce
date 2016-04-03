namespace Engine.Interface
{
    internal interface ICollidable
    {
        float Mass { get; }
        bool Fixed { get; }
    }
}