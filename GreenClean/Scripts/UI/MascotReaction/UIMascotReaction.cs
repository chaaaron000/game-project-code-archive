using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class UIMascotReaction : MonoBehaviour
{
    [SerializeField]
    private GameProgressBlackboard blackboard;

    [SerializeField]
    private UIMascotReactionView view;

    private bool tableLoaded = false;
    private MascotReactionTable reactionTable;
    private MascotReactionData lastReaction;

    private void OnEnable()
    {
        blackboard.ValueChanged += EvaluateReaction;
    }

    private void Start()
    {
        Resources
            .LoadAsync<TextAsset>(ResourceAssetPath.MascotReactionTable)
            .ToUniTask()
            .ContinueWith(jsonAsset =>
            {
                var json = jsonAsset as TextAsset;
                if (json == null)
                {
                    DebugConsole.LogError(
                        "[UIMascotReaction] Failed to load mascot reaction table."
                    );
                    return;
                }

                reactionTable = JsonConvert.DeserializeObject<MascotReactionTable>(
                    json.text,
                    MascotReactionTable.JsonConvertSettings
                );

                tableLoaded = true;
                // view.Show(reactionTable.reactions.GetRandom());
            })
            .Forget();
    }

    private void OnDisable()
    {
        blackboard.ValueChanged -= EvaluateReaction;
    }

    private void EvaluateReaction()
    {
        if (!tableLoaded)
        {
            DebugConsole.LogWarning("[UIMascotReaction] Mascot reaction table is not loaded.");
            return;
        }

        if (!TrySelectReaction(out MascotReactionData reaction))
        {
            DebugConsole.Log("[UIMascotReaction] No reaction found.");
            return;
        }

        // if (reaction == lastReaction)
        // {
        //     return;
        // }

        lastReaction = reaction;
        view.Show(reaction);
    }

    private bool TrySelectReaction(out MascotReactionData reaction)
    {
        reaction = reactionTable
            .reactions.Where(reaction => reaction.condition.Evaluate(blackboard))
            .OrderByDescending(reaction => reaction.priority)
            .Reverse() // 오름차순 정렬
            .FirstOrDefault();

        return reaction != null;
    }
}
