using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    const float speed = 5f;
    public const float InteractuableDistance = 2f;
    public const float AtractDistance = 3f;

    public static Transform tf;
    public static SpriteRenderer SR;
    public static Rigidbody2D Rb;
    Animator anim;

    static public bool CanMove;

    static public bool ChangeScenery;

    Vector2 direccion;
    public int fotograma;

    public Sprite[] vSpriteFrente;
    public Sprite[] vSpriteEspalda;
    public Sprite[] vSpriteIzquierda;
    public Sprite[] vSpriteDerecha;

    static public bool walking;


    public static bool moverse = false;
    private Vector3 inicio;
    private Vector3 fin;
    private float distancePerFrame;
    private Vector2 dir;

    public static bool BloquearMovimiento;

    const float intervalo = 0.35f; // 0,35 segundos entre pasos
    private float timer = 0f;
    public AudioSource paso;

    public AudioSource sonidoobjeto;

    public static int golpes = 0;
    public static bool invencible = false;

    public AudioSource daño;

    List<Collider2D> enemigosDentro = new List<Collider2D>();

    void Awake(){
        tf = transform;
        SR = GetComponent<SpriteRenderer>();
        Rb = GetComponent<Rigidbody2D>();
    }

    void Start(){
        anim = GetComponent<Animator>();
        walking = false;
        fotograma = 0;
        direccion = Vector2.down;
        CanMove = true;
        ChangeScenery = false;
        BloquearMovimiento = false;

        StartCoroutine(AcabaDeLlegar());
    }

    IEnumerator AcabaDeLlegar(){
        yield return null;
        GetComponent<EventTrigger>().Dialogo();
    }

    void Update(){
        float MovementX = Input.GetAxisRaw("Horizontal") * speed;
        float MovementY = Input.GetAxisRaw("Vertical") * speed;

        if(CanMove){
            MovementHandler(MovementX,MovementY);
        }else{
            Rb.linearVelocity = Vector2.zero;
        }

        if(!BloquearMovimiento){
            if(PulsandoTecla() && (!PulsandoTeclasContrarias('X') || !PulsandoTeclasContrarias('Y'))){
                walking = true;
                anim.SetBool("walking",walking);
                ManageSprite(direccion,fotograma);
            }else{
                walking = false;
                anim.SetBool("walking",walking);
                ManageSprite(direccion,0);
            }
        }

        if(moverse){
            inicio = transform.position;
            gameObject.transform.position = Vector2.MoveTowards(inicio, fin, distancePerFrame*Time.deltaTime);

            moverse = transform.position != fin ? true : false;
            
            anim.SetBool("walking",moverse);
            AsignarSprite(dir,fotograma);
        }

        if (walking || moverse){
            timer -= Time.deltaTime;       // reduce el temporizador
            if (timer <= 0f){              // cuando llega a 0, reproducimos
                paso.Play();               // o paso.PlayOneShot(paso.clip) si quieres
                timer = intervalo;         // reinicia el temporizador
            }
        }else{
            timer = 0f; // reinicia el temporizador cuando el jugador para
        }

        if(enemigosDentro.Count > 0 && !invencible){
            StartCoroutine(Invencibilidad());
        }
    }

    void ManageSprite(Vector2 dir, int fot){
        direccion = ObtenerDireccion();
        if(direccion == Vector2.up){
            SR.sprite = vSpriteEspalda[fot];
        }
        if(direccion == Vector2.down){
            SR.sprite = vSpriteFrente[fot];
        }
        if(direccion == Vector2.left){
            SR.sprite = vSpriteIzquierda[fot];
        }
        if(direccion == Vector2.right){
            SR.sprite = vSpriteDerecha[fot];
        }
    }

    void AsignarSprite(Vector2 dir, int fot){
        if (dir == Vector2.up) {
            SR.sprite = vSpriteEspalda[fot];
        }
        if (dir == Vector2.down) {
            SR.sprite = vSpriteFrente[fot];
        }
        if(dir == Vector2.left) {
            SR.sprite = vSpriteIzquierda[fot];
        }
        if(dir == Vector2.right) {
            SR.sprite = vSpriteDerecha[fot];
        }
    }

    public Vector2 ObtenerDireccion(){
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){ //Movimiento hacia arriba
            return Vector2.up;
        }else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){ //Movimiento hacia debajo
            return Vector2.down;
        }else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){ //Movimiento hacia la derecha
            return Vector2.right;
        }else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){ //Movimiento hacia la izquierda
            return Vector2.left;
        }
        return direccion;
    }



    //Manejador de movimiento del jugador
    void MovementHandler(float MovementX, float MovementY){

        Rb.linearVelocity = new Vector2(MovementX,MovementY);
        
        //Parar cuando se pulsen las teclas de direcciones opuestas
        if(PulsandoTeclasContrarias('X')){
            Rb.linearVelocity = new Vector2(0f,Rb.linearVelocity.y);
        }
        if(PulsandoTeclasContrarias('Y')){
            Rb.linearVelocity = new Vector2(Rb.linearVelocity.x,0f);
        }

        //Movimiento en diagonal
        if(Diagonal()){
            Rb.linearVelocity = new Vector2(MovementX,MovementY) / Mathf.Sqrt(2);
        }
        
    }


    //Se le pasa un char, 'X' o 'Y' dependiendo del Axis a comprobar
    //Devuelve true si se estan pulsando las teclas de direcciones contrarias del Axis especificado
    bool PulsandoTeclasContrarias(char Axis){
        if(Axis == 'X'){
            if((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))){
                return true;
            }
        }
        if(Axis == 'Y'){
            if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))){
                return true;
            }
        }
        return false;
    }


    //Devuelve true si se está pulsando alguna tecla de dirección
    bool PulsandoTecla(){
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)){
            return true;
        }
        return false;
    }


    //Devuelve true si está caminando en una diagonal
    bool Diagonal(){
        if(!PulsandoTeclasContrarias('X') && !PulsandoTeclasContrarias('Y')){
            if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))){
                return true;
            }
            if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))){
                return true;
            }
            if((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))){
                return true;
            }
            if((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))){
                return true;
            }
        }
        return false;
    }


    //Se le pasa un Vector3, el transform.position de un objeto y un float, la distancia de interacción
    //Devuelve true si el personaje está dentro del rango de distancia con el objeto
    static public bool InRange(Vector3 Interactuable, float LimitDistance){
        Vector2 distance = tf.position - Interactuable;

        if(Mathf.Abs(distance.x) < LimitDistance && Mathf.Abs(distance.y) < LimitDistance){
            return true;
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.collider.CompareTag("OutOfBoundsDoor")){
            ChangeScenery = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.layer == LayerMask.NameToLayer("Esqueleto")){
            enemigosDentro.Add(collision);
        }
        if(collision.GetComponent<Collider2D>().CompareTag("Objeto")){
            InventoryData InventoryData = FindFirstObjectByType<InventoryData>();
            if(!InventoryData.InventoryFull()){
                Objeto objeto = collision.GetComponent<Objeto>();
                if(!objeto.OnGround) return; //Bandera para que no entre 2 veces
                objeto.OnGround = false;
                bool bandera = false;
                int i = 0;

                while(i < InventoryData.numSlots && !bandera){
                    if(InventoryData.SameItem(objeto,InventoryData.vSlots[i].objeto)){
                        bandera = true;
                    }
                    i++;
                }
                i--;

                if(bandera){
                    InventoryData.vSlots[i].objeto.cantidad += objeto.cantidad;
                }else{
                    i = 0;
                    while(InventoryData.vSlots[i].objeto.id != 0){
                        i++;
                    }
                    Objeto objInventario = InventoryData.vSlots[i].objeto;
                    InventoryData.CopiarObjeto(objInventario,objeto);
                }
                Destroy(objeto.gameObject);
                sonidoobjeto.Play();
                InventoryData.ObtenerSlots();
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision){
        if(collision.gameObject.layer == LayerMask.NameToLayer("Esqueleto") && !invencible){
            StartCoroutine(Invencibilidad());
        }
    }

    void OnTriggerExit2D(Collider2D collision){
        enemigosDentro.Remove(collision);
    }

    public void Moverse(Vector3 i, Vector3 f, float d){
        moverse = true;
        inicio = i;
        fin = f;
        distancePerFrame = d;
        dir = ComprobarDireccion();
        direccion = dir;
    }

    Vector2 ComprobarDireccion(){
        Vector3 diferencia = fin - inicio;

        if(diferencia.y > 0){ //Movimiento hacia arriba
            return Vector2.up;
        }else if(diferencia.y < 0){ //Movimiento hacia debajo
            return Vector2.down;
        }else if(diferencia.x > 0){ //Movimiento hacia la derecha
            return Vector2.right;
        }else{ //Movimiento hacia la izquierda
            return Vector2.left;
        }
    }

    public void Girarse(Vector2 d){
        dir = d;
        AsignarSprite(dir,0);
    }


    private IEnumerator Invencibilidad() {
        float tiempo = 0f;
        float duracionInvencibilidad = 3f;
        float intervaloParpadeo = 0.2f;

        golpes++;
        invencible = true;
        daño.Play();

        if(golpes == 3){
            golpes = 0;
            GetComponent<EventTrigger>().Dialogo();
        }

        while (tiempo < duracionInvencibilidad) {
            SR.enabled = !SR.enabled; // alterna visibilidad
            yield return new WaitForSeconds(intervaloParpadeo);
            tiempo += intervaloParpadeo;
        }

        SR.enabled = true; // aseguramos que quede visible al final
        invencible = false;

    }
}
