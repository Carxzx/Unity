using UnityEngine;

public class Customer : MonoBehaviour
{
    Vector3 inicio;
    Vector3 destino1;
    Vector3 destino2;
    const float distancePerFrame = 0.03f;
    //const float distancePerFrame = 0.01f; //de prueba

    bool Canvas;
    bool finish;

    protected virtual void Start(){
        //Inicializamos posicion
        transform.position = new Vector2(-15f,0f);

        destino1 = new Vector3(0,0,0);
        destino2 = new Vector3(15f,0,0);
        //destino2 = new Vector3(0,-15f,0); //de prueba

        Canvas = false;
        finish = false;
    }

    protected virtual void Update(){
        if(transform.position != destino1 && !finish){
            Acercarse();
            Canvas = true; ////
        }
        if(Input.GetKeyDown(KeyCode.L) && transform.position == destino1){
            finish = true;
        }
        if(finish){
            Alejarse();
            Destroy(gameObject,2f);
        }
        Debug.Log("hola");
    }

    void Acercarse(){
        inicio = transform.position;
        transform.position = Vector2.MoveTowards(inicio, destino1, distancePerFrame);
    }

    void Alejarse(){
        inicio = transform.position;
        transform.position = Vector2.MoveTowards(inicio, destino2, distancePerFrame);
    }

}
