using UnityEngine;
using System.Collections;

public class Pico : MonoBehaviour
{
float rotacionInicial = 30f;
    const int rotacion = 90;
    const float tiempoRotacion = 0.3f;

    void Start(){
        transform.position = new Vector2(Player.tf.position.x, Player.tf.position.y+0.8f);
        if(!Player.SR.flipX){
            rotacionInicial*=-1;
        }

        //Girar el sprite sin depender del pivot
        Vector3 escala = transform.localScale;
        escala.x = Player.SR.flipX ? 1f : -1f;
        transform.localScale = escala;


        transform.rotation = Quaternion.Euler(0f, 0f, rotacionInicial);

        StartCoroutine(RotarGradual(rotacion,tiempoRotacion));
    }

    private IEnumerator RotarGradual(float grados, float tiempoDeRotacion){
        Player.CanMove = false;

        Quaternion objetivo;
        if(Player.SR.flipX){
            objetivo = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x,gameObject.transform.rotation.eulerAngles.y,gameObject.transform.rotation.eulerAngles.z + grados);
        }else{
            objetivo = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x,gameObject.transform.rotation.eulerAngles.y,gameObject.transform.rotation.eulerAngles.z - grados);
        }
        
        float velocidadRotacion = grados / tiempoDeRotacion;
        float tiempoTranscurrido = 0f;

        while(tiempoTranscurrido < tiempoDeRotacion){
            // Rotar gradualmente hacia el objetivo
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, objetivo, velocidadRotacion * Time.deltaTime);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;  // Esperar al siguiente frame
        }

        gameObject.transform.rotation = objetivo;

        Destroy(gameObject);
        
        Player.CanMove = true;
    }

    
    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("PiedraTrigger")){
            PiedraScript piedra = collision.transform.parent.GetComponent<PiedraScript>();

            piedra.vidaPiedra--;
            if(piedra.vidaPiedra <= 0){
                piedra.DestruirPiedra();
            }
        }
    }
    
}
