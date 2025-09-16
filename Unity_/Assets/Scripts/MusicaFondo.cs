using UnityEngine;

public class MusicaFondo : MonoBehaviour
{
    public AudioClip loopClip;     // El audio a repetir
    private AudioSource audioSource;

    public bool detenerSonido = false; // Bool que controla el loop

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = loopClip;
        audioSource.loop = true; // Lo dejamos en bucle
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
