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
    int direccion;

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

    int ComprobarDireccion(){
        Vector3 diferencia = fin - inicio;

        if(diferencia.y < 0){ //Movimiento hacia arriba
            return 1;
        }else if(diferencia.y > 0){ //Movimiento hacia debajo
            return 2;
        }else if(diferencia.x > 0){ //Movimiento hacia la derecha
            return 3;
        }else{ //Movimiento hacia la izquierda
            return 4;
        }
    }

    void AsignarSprite(int dir, int fot){
        switch(dir){
            case 1:
                GetComponent<SpriteRenderer>().sprite = arriba[fot];
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = abajo[fot];
                break;
            case 3:
            case 4:
                GetComponent<SpriteRenderer>().flipX = direccion == 3 ? false : true;
                GetComponent<SpriteRenderer>().sprite = lado[fot];
                break;
        }
    }

    public void Moverse(Vector3 i, Vector3 f, float d){
        walking = true;
        inicio = i;
        fin = f;
        distancePerFrame = d;
        direccion = ComprobarDireccion();
    }
}
