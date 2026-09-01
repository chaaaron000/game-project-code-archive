using UnityEngine;

public static class ShaderProps
{
    public static readonly int CHANGE_TARGET_TEXTURE = Shader.PropertyToID(
        "_Change_Target_Texture"
    );

    public static readonly int CIRCLE_RADIUS = Shader.PropertyToID("_Circle_Radius");

    public static readonly int CIRCLE_SOFTNESS = Shader.PropertyToID("_Circle_Softness");
}
