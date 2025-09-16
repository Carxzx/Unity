using UnityEngine;

public class MusicaFondo : MonoBehaviour
{
    public AudioSource audioSource;

    public bool detenerSonido = false; // Bool que controla el loop

    void Start()
    {
        audioSource.Play();      // Empieza a sonar
    }

    void Update()
    {
        if (detenerSonido && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        else if (!detenerSonido && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
