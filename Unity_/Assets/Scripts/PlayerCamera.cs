using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    const float RaycastDistanceX = 12.45f;
    const float RaycastDistanceY = 7f;

    public Transform tf;
    public GameObject jugador;

    Vector2 OriginRaycast;
    RaycastHit2D R_left, R_up, R_right, R_down;

    int layerMask;
    
    void Start(){
        //La camara comienza en la posicion del jugador
        tf.position = new Vector3(jugador.transform.position.x, jugador.transform.position.y, tf.position.z);

        layerMask = LayerMask.GetMask("OutOfBounds");

        //Obtenemos los Raycasts y Inicializamos la cámara
        ObtenerRaycasts();
        InicializarCamara();
    }

    void Update(){
        //Obtenemos los Raycasts
        ObtenerRaycasts();

        //Actualizamos la posicion de la cámara con la del jugador
        Vector2 auxpos = tf.position;
        tf.position = new Vector3(jugador.transform.position.x, jugador.transform.position.y, tf.position.z);

        //Si se choca con alguna pared, la cámara se queda en su sitio para no mostrar la pared
        if(RaycastPared(R_left) || RaycastPared(R_right)){
            tf.position = new Vector3(auxpos.x,tf.position.y,tf.position.z);
        }
        if(RaycastPared(R_up) || RaycastPared(R_down)){
            tf.position = new Vector3(tf.position.x,auxpos.y,tf.position.z);
        }

        if(Door.TP){
            Door.TP = false;
            RestartCamera();
        }
    }


    //Obtiene la posicion del jugador y crea los Raycast en cada dirección a partir de esa posición
    void ObtenerRaycasts(){
        OriginRaycast = jugador.transform.position;

        R_left = Physics2D.Raycast(OriginRaycast,Vector2.left,RaycastDistanceX,layerMask);
        R_up = Physics2D.Raycast(OriginRaycast,Vector2.up,RaycastDistanceY,layerMask);
        R_right = Physics2D.Raycast(OriginRaycast,Vector2.right,RaycastDistanceX,layerMask);
        R_down = Physics2D.Raycast(OriginRaycast,Vector2.down,RaycastDistanceY,layerMask);
    }


    //Ajusta toda la cámara en su conjunto para que no se vean las paredes OutOfBounds
    void InicializarCamara(){
        R_left = AjustarRaycast(R_left,Vector2.left,RaycastDistanceX);
        R_up = AjustarRaycast(R_up,Vector2.up,RaycastDistanceY);
        R_right = AjustarRaycast(R_right,Vector2.right,RaycastDistanceX);
        R_down = AjustarRaycast(R_down,Vector2.down,RaycastDistanceY);
    }


    //Recibe un Raycast, una dirección (la del Raycast) y una distancia (la del Raycast)
    //Devuelve el Raycast corregido, además de actualizar la posición de inicio de todos los Raycasts
    RaycastHit2D AjustarRaycast(RaycastHit2D Raycast, Vector2 direccion, float RaycastDistance){
        if(RaycastPared(Raycast)){
            while(RaycastPared(Raycast)){
                OriginRaycast -= direccion * 0.01f;
                Raycast = Physics2D.Raycast(OriginRaycast,direccion,RaycastDistance,layerMask); //empieza afuera
                tf.position = new Vector3(OriginRaycast.x,OriginRaycast.y,tf.position.z);
            }
            OriginRaycast += direccion * 0.01f;
            Raycast = Physics2D.Raycast(OriginRaycast,direccion,RaycastDistance,layerMask);
            tf.position = new Vector3(OriginRaycast.x,OriginRaycast.y,tf.position.z);
        }
        return Raycast;
    }


    //Recibe un Raycast
    //Devuelve true si el Raycast ha detectado una pared OutOfBounds que no se debe mostrar en pantalla
    bool RaycastPared(RaycastHit2D Raycast){
        return Raycast.collider != null;
    }


    void RestartCamera(){
        Start();
    }

    //Debug.DrawRay(OriginRaycastLeft, Vector2.right * RaycastDistanceX, Color.red); //Pintar Raycast
    //Debug.DrawRay(OriginRaycastUp, Vector2.down * RaycastDistanceY, Color.green); //Pintar Raycast
    //Debug.DrawRay(OriginRaycastRight, Vector2.left * RaycastDistanceX, Color.yellow); //Pintar Raycast
    //Debug.DrawRay(OriginRaycastDown, Vector2.up * RaycastDistanceY, Color.blue); //Pintar Raycast
}

