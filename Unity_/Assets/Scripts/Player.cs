using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    const float speed = 5f;
    public const float InteractuableDistance = 2f;
    public const float AtractDistance = 3f;

    public static Transform tf;
    public static SpriteRenderer SR;
    public static Rigidbody2D Rb;

    static public bool CanMove;

    static public bool ChangeScenery;
    static public Collision2D collisionAux;

    public Sprite[] vSprite; //0 = ARRIBA, 1 = DERECHA, 2 = ABAJO

    void Awake(){
        tf = transform;
        SR = GetComponent<SpriteRenderer>();
        Rb = GetComponent<Rigidbody2D>();
    }

    void Start(){
        CanMove = true;
        ChangeScenery = false;
    }

    
    void Update(){
        float MovementX = Input.GetAxisRaw("Horizontal") * speed;
        float MovementY = Input.GetAxisRaw("Vertical") * speed;

        if(CanMove){
            MovementHandler(MovementX,MovementY);
        }else{
            Rb.linearVelocity = new Vector2(0,0);
        }

        ManageSprite();
    }

    void ManageSprite(){
        if(Input.GetKey(KeyCode.W)){
            SR.sprite = vSprite[0];
        }
        if(Input.GetKey(KeyCode.S)){
            SR.sprite = vSprite[2];
        }
        if(Input.GetKey(KeyCode.A)){
            SR.sprite = vSprite[1];
            SR.flipX = true;
        }
        if(Input.GetKey(KeyCode.D)){
            SR.sprite = vSprite[1];
            SR.flipX = false;
        }
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
            if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)){
                return true;
            }
        }
        if(Axis == 'Y'){
            if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)){
                return true;
            }
        }
        return false;
    }


    //Devuelve true si se está pulsando alguna tecla de dirección
    bool PulsandoTecla(){
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)){
            return true;
        }
        return false;
    }


    //Devuelve true si está caminando en una diagonal
    bool Diagonal(){
        if(!PulsandoTeclasContrarias('X') && !PulsandoTeclasContrarias('Y')){
            if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)){
                return true;
            }
            if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)){
                return true;
            }
            if(Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)){
                return true;
            }
            if(Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)){
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
            collisionAux = collision;
            ChangeScenery = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.GetComponent<Collider2D>().CompareTag("Objeto")){
            InventoryData InventoryData = FindFirstObjectByType<InventoryData>();
            if(!InventoryData.InventoryFull()){
                Objeto objeto = collision.GetComponent<Objeto>();
                bool bandera = false;
                int i = 0;

                while(i < InventoryData.numSlots && !bandera){
                    if(InventoryData.SameID(objeto.id,InventoryData.vSlots[i].objeto.id)){
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
                    Slot slot = InventoryData.vSlots[i];
                    Objeto objInv = slot.objeto;

                    objInv.id = objeto.id;
                    objInv.cantidad = objeto.cantidad;
                    objInv.OnGround = false;

                    slot.ActualizarSprite(objInv);
                }
                Destroy(objeto.gameObject);
                InventoryData.ObtenerSlots();
            }
        }
    }
}
