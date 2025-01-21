[System.Serializable]
public struct Voxel
{
    public bool isActive;
    public byte type;  // Using byte instead of enum

    // Static byte constants for block types - gives us enum-like readability
    public static class Type
    {
        public const byte Air = 0;
        public const byte Stone = 1;
        // Easy to add more:
        // public const byte Dirt = 2;
        // public const byte Grass = 3;
        // public const byte Wood = 4;
        // etc...
    }

    public Voxel(byte type, bool isActive = true)
    {
        this.type = type;
        this.isActive = isActive;
    }

    // Convenience constructor that maintains readable code
    public static Voxel Create(byte type, bool isActive = true)
    {
        return new Voxel(type, isActive);
    }
}