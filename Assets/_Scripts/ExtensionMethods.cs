using UnityEngine;

public static class ExtensionMethods
{

    public static float Remap(float value, Vector2 from, Vector2 to)
    {
        return to.x + (value - from.x) * (to.y - to.x) / (from.y - from.x);
        //return (value - from.x) / (to.x - from.x) * (to.y - from.y) + from.y;
    }

}

