using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EncenderLuz : MonoBehaviour
{
    const int Encender = 20;
    const int Apagar = 8;
    Light2D luz;
    Clock Clock;

    Animator anim;

    void Start(){
        anim = GetComponent<Animator>();
        Clock = FindFirstObjectByType<Clock>();
        luz = GetComponent<Light2D>();
    }

    void Update(){
        if(Clock.hour == Encender){
            luz.enabled = true;
        }
        if(Clock.hour == Apagar){
            luz.enabled = false;
        }

        anim.SetBool("onFire",luz.enabled);
    }
}
