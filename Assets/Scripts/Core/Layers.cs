namespace ARKitect.Core
{
    /// <summary>
    /// Builtin and custom layers used by Unity to restrict operations such as raycasting and rendering
    /// </summary>
    public enum Layers
    {
        // Unity default builtin layers
        DEFAULT = 0,
        TRANSPARENT_FX = 1,
        IGNORE_RAYCAST = 2,
        WATER = 4,
        UI = 5,

        // ARkitect custom layers
        GRID = 3,
        BUILDING_OBJECT = 6
    }
}