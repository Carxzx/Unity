using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public UnityEvent evento;

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("PlayerTrigger")){
            evento.Invoke();
            gameObject.SetActive(false);
        }
    }
}
