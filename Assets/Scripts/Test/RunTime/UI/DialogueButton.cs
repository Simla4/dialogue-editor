using UnityEngine;

public class DialogueButton : MonoBehaviour
{
    public void OnClickDialogueButton()
    {
        DialogueLoader.OnCallDialogueData?.Invoke(gameObject.name);
    }
}
