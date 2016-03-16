namespace Engine
{
    public class Constants
    {
        public const string ContentFolder = "Content";
        public const string DefaultNamespace = "Engine";
    }

    public enum State
    {
        Solid, Passive
    }

    public enum DrawType
    {
        UnderLay, OverLay
    }

    public enum TileType
    {
        Map,
        Autotile,
        Event
    }
}