using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Espada : MonoBehaviour
{
    float rotacionInicial = 30f;
    const int rotacion = 179;
    const float tiempoRotacion = 0.3f;

    const float RaycastDistance = 1f; ///////////////////////////////////////////////////////////////////////////////////// 1f
    int layerMask;

    Vector2 direccion;

    Player Player;

    public AudioClip[] clips; // Arrastras varios sonidos en el Inspector
    public AudioSource audioSource;

    public AudioClip sonidodaño; // Arrastras varios sonidos en el Inspector
    public AudioSource audiodaño;

    void Start() {
        Player = FindFirstObjectByType<Player>();
        layerMask = LayerMask.GetMask("EsqueletoTrigger");
        //Debug.LogError("layerMask: " + layerMask);


        Vector2 offset = new Vector2(0, 0.8f);
        float rotacionZ = 0f;

        direccion = Player.ObtenerDireccion();

        switch (direccion) {
            case Vector2 v when v == Vector2.right:
                //offset = new Vector2(0.8f, 0.2f);
                rotacionInicial = 35f;
                break;

            case Vector2 v when v == Vector2.left:
                //offset = new Vector2(-0.8f, 0.2f);
                rotacionInicial = 35f;
                break;

            case Vector2 v when v == Vector2.up:
                //offset = new Vector2(0f, 1f);
                rotacionInicial = 305f;
                break;

            case Vector2 v when v == Vector2.down:
                //offset = new Vector2(0f, -1f);
                rotacionInicial = 305f;
                break;
        }

        // Posición del pico
        transform.position = (Vector2)Player.tf.position + offset;

        transform.rotation = Quaternion.Euler(0f, 0f, rotacionInicial);

        StartCoroutine(RotarGradual(rotacion, tiempoRotacion));
    }

    private IEnumerator RotarGradual(float grados, float tiempoDeRotacion){
        Player.CanMove = false;

        Quaternion objetivo;

        float signo = (direccion == Vector2.right || direccion == Vector2.down) ? -1f : 1f;

        objetivo = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x,gameObject.transform.rotation.eulerAngles.y,gameObject.transform.rotation.eulerAngles.z + grados * signo);

        float velocidadRotacion = grados / tiempoDeRotacion;
        float tiempoTranscurrido = 0f;

        ReproducirAleatorio();

        bool entre = false;

        while(tiempoTranscurrido < tiempoDeRotacion){
            // Rotar gradualmente hacia el objetivo
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, objetivo, velocidadRotacion * Time.deltaTime);
            tiempoTranscurrido += Time.deltaTime;
            if(tiempoTranscurrido > tiempoDeRotacion/2 && !entre){
                ComprobarGolpe();
                entre = true;
            }
            yield return null;  // Esperar al siguiente frame
        }

        gameObject.transform.rotation = objetivo;

        Destroy(gameObject); //Eliminar la espada de la escena
        
        Player.CanMove = true;
    }

    
    void ComprobarGolpe(){
        ///Obtener la direccion a la que mira el personaje
        Vector2 direccion = Player.ObtenerDireccion();
        Vector2 posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.5f);

        HashSet<Collider2D> collidersGolpeados = new HashSet<Collider2D>();


        RaycastHit2D[] raycasts1 = Physics2D.RaycastAll(posicion, direccion, RaycastDistance, layerMask);
        //Debug.Log("Raycast1: " + raycasts1[0]);
        Debug.DrawRay(posicion, direccion * RaycastDistance, Color.red);

        if(direccion == Vector2.up || direccion == Vector2.down){
            posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.5f) + new Vector2(0.5f,0f);
        }else{
            posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.5f) + new Vector2(0f,0.7f);
        }
        RaycastHit2D[] raycasts2 = Physics2D.RaycastAll(posicion,direccion,RaycastDistance,layerMask);
        //Debug.Log("Raycast2: " + raycasts2[0]);
        Debug.DrawRay(posicion, direccion * RaycastDistance, Color.red);

        if(direccion == Vector2.up || direccion == Vector2.down){
            posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.5f) - new Vector2(0.5f,0f);
        }else{
            posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.5f) - new Vector2(0f,0.7f);
        }
        RaycastHit2D[] raycasts3 = Physics2D.RaycastAll(posicion,direccion,RaycastDistance,layerMask);
        //Debug.Log("Raycast3: " + raycasts3[0]);
        Debug.DrawRay(posicion, direccion * RaycastDistance, Color.red);


        foreach (var hit in raycasts1) {
            if (hit.collider != null && collidersGolpeados.Add(hit.collider)) {
                GolpearEsqueleto(hit.collider, direccion);
            }
        }
        foreach (var hit in raycasts2) {
            if (hit.collider != null && collidersGolpeados.Add(hit.collider)) {
                GolpearEsqueleto(hit.collider, direccion);
            }
        }
        foreach (var hit in raycasts3) {
            if (hit.collider != null && collidersGolpeados.Add(hit.collider)) {
                GolpearEsqueleto(hit.collider, direccion);
            }
        }

    }


    void GolpearEsqueleto(Collider2D collision, Vector2 direccion){
        Esqueleto esqueleto = collision.transform.parent.GetComponent<Esqueleto>();
        audiodaño.PlayOneShot(sonidodaño);

        esqueleto.vida = esqueleto.vida -2;
        esqueleto.Retroceder(direccion);

        if(esqueleto.vida <= 0){
            esqueleto.DestruirEsqueleto();
        }
    }

    public void ReproducirAleatorio()
    {
        if (clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[index]);
    }
}
