using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    /*
    const float RaycastDistanceX = 12.5f;
    const float RaycastDistanceY = 7f;

    public Transform tf;

    GameObject jugadorObject;
    Player jugador;
    Transform jugador_tf;
    
    void Start(){

        //Obtener el Objeto Jugador
        jugadorObject = GameObject.FindWithTag("Player");

        //Si existe, guarda sus componentes
        if(jugadorObject != null){
            jugador = jugadorObject.GetComponent<Player>();
            jugador_tf = jugador.transform;
        }

        //Usamos un Raycast en cada direccion para recolocar la camara en caso de que haya una pared
        Vector2 OriginRaycast = jugador_tf;

        RaycastHit2D R_left = Physics2D.Raycast(OriginRaycast,Vector2.left,RaycastDistanceX);
        RaycastHit2D R_up = Physics2D.Raycast(OriginRaycast,Vector2.up,RaycastDistanceY);
        RaycastHit2D R_right = Physics2D.Raycast(OriginRaycast,Vector2.right,RaycastDistanceX);
        RaycastHit2D R_down = Physics2D.Raycast(OriginRaycast,Vector2.down,RaycastDistanceY);

        Debug.DrawRay(OriginRaycast, Vector2.left * RaycastDistanceX, Color.red); //Pintar Raycast
        Debug.DrawRay(OriginRaycast, Vector2.up * RaycastDistanceY, Color.green); //Pintar Raycast
        Debug.DrawRay(OriginRaycast, Vector2.right * RaycastDistanceX, Color.yellow); //Pintar Raycast
        Debug.DrawRay(OriginRaycast, Vector2.down * RaycastDistanceX, Color.purple); //Pintar Raycast


        //La camara comienza en la posicion del jugador
        tf.position = jugador_tf;
    }

    void Update(){

        //Actualizamos la posicion de la camara con la del jugador
        tf.position = jugador_tf;
    }

    //Debug.DrawRay(OriginRaycast, direction * RaycastDistance, Color.red);

    */
}

