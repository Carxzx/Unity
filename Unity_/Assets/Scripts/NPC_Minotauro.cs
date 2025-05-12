using UnityEngine;

public class NPC_Minotauro : MonoBehaviour
{
    Rigidbody2D Rb;
    Animator anim;
    public int fotograma;

    public Sprite[] lado;
    public Sprite[] arriba;
    public Sprite[] abajo;
    bool walking;
    Vector2 direccion;

    Vector3 inicio;
    Vector3 fin;
    float distancePerFrame;

    void Start(){
        fotograma = 0;
        anim = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        if(walking){
            inicio = transform.position;
            gameObject.transform.position = Vector2.MoveTowards(inicio, fin, distancePerFrame);

            walking = transform.position != fin ? true : false;

            anim.SetBool("Walk",walking);
            AsignarSprite(direccion,fotograma);
        }
    }

    Vector2 ComprobarDireccion(){
        Vector3 diferencia = fin - inicio;

        if(diferencia.y < 0){ //Movimiento hacia arriba
            return Vector2.up;
        }else if(diferencia.y > 0){ //Movimiento hacia debajo
            return Vector2.down;
        }else if(diferencia.x > 0){ //Movimiento hacia la derecha
            return Vector2.right;
        }else{ //Movimiento hacia la izquierda
            return Vector2.left;
        }
    }

    void AsignarSprite(Vector2 dir, int fot){
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (dir == Vector2.up) {
            sr.sprite = arriba[fot];
        } else if (dir == Vector2.down) {
            sr.sprite = abajo[fot];
        } else if (dir == Vector2.right || dir == Vector2.left) {
            sr.flipX = (dir == Vector2.right) ? false : true;
            sr.sprite = lado[fot];
        }
    }

    public void Moverse(Vector3 i, Vector3 f, float d){
        walking = true;
        inicio = i;
        fin = f;
        distancePerFrame = d;
        direccion = ComprobarDireccion();
    }

    public void Girarse(Vector2 direccion){
        AsignarSprite(direccion,0);
    }
}
