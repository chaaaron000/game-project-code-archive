using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIMascotReactionView : MonoBehaviour
{
    [SerializeField]
    private RectTransform reactionPanel;

    [SerializeField]
    private TMP_Text messageText;

    [SerializeField]
    private float moveDuration = 1f;

    [SerializeField]
    private float waitDuration = 2f;

    private MascotReactionData currentReaction;
    private Sequence sequence;
    private float panelHeight;

    private void Awake()
    {
        panelHeight = reactionPanel.rect.height;
        reactionPanel.SetAnchoredYPos(0f);
    }

    public void Show(MascotReactionData reaction)
    {
        if (currentReaction != null)
        {
            if (currentReaction == reaction)
            {
                return;
            }

            if (currentReaction.priority > reaction.priority)
            {
                return;
            }
        }

        sequence?.Kill();
        currentReaction = reaction;
        string message = reaction.messages.GetRandom();
        messageText.text = message;

        sequence = DOTween
            .Sequence()
            .Append(reactionPanel.DOAnchorPosY(-panelHeight, moveDuration))
            .AppendInterval(waitDuration)
            .Append(reactionPanel.DOAnchorPosY(0f, moveDuration))
            .OnKill(() =>
            {
                reactionPanel.SetAnchoredYPos(0f);
                currentReaction = null;
            });
    }
}
