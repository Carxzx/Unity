using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public UnityEvent evento;
    public bool DisableAfterFinishing;
    public bool TriggerActivado;
    public int id; //Para la altura de la burbuja

    UI_Handler UI_Handler;

    void Start(){
        UI_Handler = FindFirstObjectByType<UI_Handler>();
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("PlayerTrigger") && TriggerActivado){
            UI_Handler.EventOn();
            evento.Invoke();
            if(DisableAfterFinishing){
                gameObject.SetActive(false);
            }
        }
    }

    public void Dialogo(){
        UI_Handler.EventOn();
        evento.Invoke();
        if(DisableAfterFinishing){
            gameObject.SetActive(false);
        }
    }
}