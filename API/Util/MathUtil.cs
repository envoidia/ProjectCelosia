using Microsoft.Xna.Framework;

namespace API.Util;

public static class MathUtil {
    public static Vector2 SmoothStep(Vector2 v1, Vector2 v2, float amt) {
        return new Vector2(MathHelper.SmoothStep(v1.X, v2.X, amt),
        MathHelper.SmoothStep(v1.Y, v2.Y, amt));
    }
}