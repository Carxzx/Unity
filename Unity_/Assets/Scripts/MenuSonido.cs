using UnityEngine;
using System.Collections;

public class MenuSonido : MonoBehaviour
{
    public AudioSource audioSource;

    public bool onEnable;
    [HideInInspector]
    public bool primeravez = true;

    void Start(){
        primeravez = true;
    }

    void OnEnable(){
        if(onEnable) audioSource.Play();
    }

    void OnDisable(){
        if(onEnable && !primeravez){
            ReproducirAudioAunqueSeApague(audioSource);
        }
        primeravez = false;
    }

    public void ReproducirAudioAunqueSeApague(AudioSource original){
        // Crear un GameObject temporal
        GameObject tempGO = new GameObject("AudioTemporal");

        // Añadir un AudioSource y copiar la configuración del original
        AudioSource tempAudio = tempGO.AddComponent<AudioSource>();
        tempAudio.clip = original.clip;
        tempAudio.volume = 0.25f;
        tempAudio.pitch = original.pitch;
        tempAudio.spatialBlend = original.spatialBlend;
        tempAudio.loop = original.loop;

        tempAudio.Play();
        Debug.Log("hola");

        // Destruir cuando termine de sonar
        Destroy(tempGO, original.clip.length+0.1f);
    }

    void PlayAudio(){
        audioSource.Play();
    }
}
