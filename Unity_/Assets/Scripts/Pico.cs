using UnityEngine;
using System.Collections;

public class Pico : MonoBehaviour
{
    float rotacionInicial = 30f;
    const int rotacion = 90;
    const float tiempoRotacion = 0.3f;

    const float RaycastDistance = 1f;
    int layerMask;

    Player Player;

    void Start(){
        Player = FindFirstObjectByType<Player>();
        layerMask = LayerMask.GetMask("PiedraTrigger");

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

        ComprobarGolpe(); //Comprobar si ha chocado contra una piedra

        Destroy(gameObject); //Eliminar el pico de la escena
        
        Player.CanMove = true;
    }

    void ComprobarGolpe(){
        ///Obtener la direccion a la que mira el personaje
        Vector2 direccion = Player.ObtenerDireccion();
        Vector2 posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+1f);

        //Castear un rayo desde el jugador a la direccion que mira
        RaycastHit2D Raycast = Physics2D.Raycast(posicion,direccion,RaycastDistance,layerMask);

        if(Raycast.collider != null){
            GolpearPiedra(Raycast.collider);
        }else{
            posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.75f);
            Raycast = Physics2D.Raycast(posicion,direccion,RaycastDistance,layerMask);
            if(Raycast.collider != null){
                GolpearPiedra(Raycast.collider);
            }else{
                posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+1.25f);
                Raycast = Physics2D.Raycast(posicion,direccion,RaycastDistance,layerMask);
                if(Raycast.collider != null){
                    GolpearPiedra(Raycast.collider);
                }
            }
        }
    }

    
    void GolpearPiedra(Collider2D collision){
        PiedraScript piedra = collision.transform.parent.GetComponent<PiedraScript>();

        piedra.vidaPiedra--;
        if(piedra.vidaPiedra <= 0){
            piedra.DestruirPiedra();
        }
    }
    
}
