using UnityEngine;
using System.Collections;

public class ChineseVoiceTalk : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip voiceClip;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);

        animator.SetTrigger("Talk");

        audioSource.clip = voiceClip;
        audioSource.Play();

        // Wait until the voice finishes
        yield return new WaitWhile(() => audioSource.isPlaying);

        // Return to idle animation
        animator.Play("Animation");
    }
}