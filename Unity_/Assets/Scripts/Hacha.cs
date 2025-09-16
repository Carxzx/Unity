using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hacha : MonoBehaviour
{
    float rotacionInicial = 30f;
    const int rotacion = 90;
    const float tiempoRotacion = 0.3f;

    const float RaycastDistance = 1f;
    int layerMask;

    Vector3 escala;
    Vector2 direccion;

    Player Player;

    public AudioClip sonido;   // arrastras el clip desde el Inspector
    public AudioSource audio;

    public AudioClip sonidotalar;   // arrastras el clip desde el Inspector
    public AudioSource audiotalar;

    void Start() {
        Player = FindFirstObjectByType<Player>();
        layerMask = LayerMask.GetMask("ArbolTrigger");


        Vector2 offset = new Vector2(0, 0.8f);
        float rotacionZ = 0f;

        direccion = Player.ObtenerDireccion();

        switch (direccion) {
            case Vector2 v when v == Vector2.right:
                //offset = new Vector2(0.8f, 0.2f);
                rotacionInicial = 305f;
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
                rotacionInicial = 215f;
                break;
        }

        if (direccion == Vector2.right || direccion == Vector2.down) {
            escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;
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

        int cont = 0;

        audio.PlayOneShot(sonido);

        while(tiempoTranscurrido < tiempoDeRotacion){
            // Rotar gradualmente hacia el objetivo
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, objetivo, velocidadRotacion * Time.deltaTime);
            if(cont == 30){
                ComprobarGolpe();
            }
            cont++;
            tiempoTranscurrido += Time.deltaTime;
            yield return null;  // Esperar al siguiente frame
        }

        gameObject.transform.rotation = objetivo;

        Destroy(gameObject); //Eliminar el pico de la escena
        
        Player.CanMove = true;
    }

    void ComprobarGolpe(){
        ///Obtener la direccion a la que mira el personaje
        Vector2 direccion = Player.ObtenerDireccion();
        Vector2 posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.5f);

        HashSet<Collider2D> collidersGolpeados = new HashSet<Collider2D>();

        //Castear un rayo desde el jugador a la direccion que mira
        RaycastHit2D Raycast1 = Physics2D.Raycast(posicion,direccion,RaycastDistance,layerMask);

        if(direccion == Vector2.up || direccion == Vector2.down){
            posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.5f) + new Vector2(0.5f,0f);
        }else{
            posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.5f) + new Vector2(0f,0.7f);
        }
        RaycastHit2D Raycast2 = Physics2D.Raycast(posicion,direccion,RaycastDistance,layerMask);

        if(direccion == Vector2.up || direccion == Vector2.down){
            posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.5f) - new Vector2(0.5f,0f);
        }else{
            posicion = new Vector2(Player.transform.position.x, Player.transform.position.y+0.5f) - new Vector2(0f,0.7f);
        }
        RaycastHit2D Raycast3 = Physics2D.Raycast(posicion,direccion,RaycastDistance,layerMask);

        if (Raycast1.collider != null && collidersGolpeados.Add(Raycast1.collider)) {
            GolpearArbol(Raycast1.collider);
            //Debug.Log("Hit a: " + hit.collider.gameObject.transform.parent.name);
        }

        if (Raycast2.collider != null && collidersGolpeados.Add(Raycast2.collider)) {
            GolpearArbol(Raycast2.collider);
            //Debug.Log("Hit a: " + hit.collider.gameObject.transform.parent.name);
        }

        if (Raycast3.collider != null && collidersGolpeados.Add(Raycast3.collider)) {
            GolpearArbol(Raycast3.collider);
            //Debug.Log("Hit a: " + hit.collider.gameObject.transform.parent.name);
        }
    }

    void GolpearArbol(Collider2D collision){
        ArbolScript arbol = collision.transform.parent.GetComponent<ArbolScript>();
        PlayAndDestroy(sonidotalar);

        arbol.vidaArbol--;
        if(arbol.vidaArbol <= 0){
            arbol.DestruirArbol();
        }
    }

    void PlayAndDestroy(AudioClip clip)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = transform.position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = 0.25f;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }
}
