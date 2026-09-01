using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MascotReactionData
{
    [SerializeReference, SubclassSelector]
    public BlackboardConditionBase condition;

    public List<string> messages = new();

    public int priority;

    public float cooldown;

    public bool oncePerStage;
}
