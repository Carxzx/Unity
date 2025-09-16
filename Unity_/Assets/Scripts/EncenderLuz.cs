using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EncenderLuz : MonoBehaviour
{
    const int Encender = 20;
    const int Apagar = 8;
    Light2D luz;
    Clock Clock;

    Animator anim;

    public AudioSource audiosource;

    bool once = false;

    const float radioMax = 8f;
    const float volumenMax = 0.4f;

    void Start(){
        anim = GetComponent<Animator>();
        Clock = FindFirstObjectByType<Clock>();
        luz = GetComponent<Light2D>();
        audiosource = GetComponent<AudioSource>();
    }

    void Update(){
        if(Clock.hour == Encender){
            luz.enabled = true;
            once = true;
        }
        if(Clock.hour == Apagar){
            luz.enabled = false;
            once = false;
            audiosource.Stop();
        }

        anim.SetBool("onFire",luz.enabled);

        if(once){
            if (!audiosource.isPlaying){
                audiosource.Play();
            }

            // Calcular distancia al oyente
            float distancia = Vector3.Distance(Player.tf.position, transform.position);

            if (distancia <= radioMax){
                // Volumen cuadrático inverso
                float t = 1f - (distancia / radioMax); // 0 cuando lejos, 1 cuando cerca
                float volumen = Mathf.Clamp01(t * t) * volumenMax;

                audiosource.volume = volumen;
            }else{
                audiosource.volume = 0f; // fuera del rango no se escucha nada
            }
        }
    }
}
