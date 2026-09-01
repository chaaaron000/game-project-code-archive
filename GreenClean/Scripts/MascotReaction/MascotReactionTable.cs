using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public sealed class MascotReactionTable
{
    public static readonly JsonSerializerSettings JsonConvertSettings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
    };

    public int version;

    public List<MascotReactionData> reactions = new();
}
