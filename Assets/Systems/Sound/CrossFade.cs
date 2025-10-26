using UnityEngine;
using System.Collections;

public class AudioCrossFader : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
    public float crossfadeDuration = 1.0f;
    public bool CrossfadeOnDeath = true;

    private void Start()
    {
        if (CrossfadeOnDeath)
            GameManager.Instance.playerHealth.OnDeath += (DamageType noe) => CrossfadeAudio();
    }
    public void CrossfadeAudio(/*AudioClip newClip*/)
    {

            StartCoroutine(FadeOutAndIn(audioSource1, audioSource2/*, newClip*/));
        StartCoroutine(FadeOutAndIn(audioSource2, audioSource3/*, newClip*/));

    }

    IEnumerator FadeOutAndIn(AudioSource fadeOutSource, AudioSource fadeInSource/*, AudioClip newClip*/)
    {
        float timer = 0f;
        float startVolumeOut = fadeOutSource.volume;
        float startVolumeIn = fadeInSource.volume; // Assuming fadeInSource might have a non-zero volume

        //fadeInSource.clip = newClip;

        while (timer < crossfadeDuration)
        {
            timer += Time.deltaTime;
            fadeOutSource.volume = Mathf.Lerp(startVolumeOut, 0f, timer / crossfadeDuration);

            yield return null;
        }

        timer = 0f;
        fadeInSource.Play();
        while (timer < crossfadeDuration )
        {
            timer += Time.deltaTime;
            fadeInSource.volume = Mathf.Lerp(0, startVolumeIn, timer  / crossfadeDuration); // Fade in to full volume
            yield return null;
        }

        fadeOutSource.Stop();
        fadeOutSource.volume = 0f; // Ensure it's completely silent
        //fadeInSource.volume = 1f; // Ensure new source is at full volume
    }
}