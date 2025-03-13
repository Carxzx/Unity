using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    const float RaycastDistanceX = 12.7f;
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

        //La camara comienza en la posicion del jugador
        tf.position = new Vector3(jugador_tf.position.x, jugador_tf.position.y, tf.position.z);

        //Usamos un Raycast en cada direccion para recolocar la camara en caso de que haya una pared
    }

    void Update(){
        Vector2 OriginRaycast = jugador_tf.position;
        RaycastHit2D R_left = Physics2D.Raycast(OriginRaycast,Vector2.left,RaycastDistanceX);
        RaycastHit2D R_up = Physics2D.Raycast(OriginRaycast,Vector2.up,RaycastDistanceY);
        RaycastHit2D R_right = Physics2D.Raycast(OriginRaycast,Vector2.right,RaycastDistanceX);
        RaycastHit2D R_down = Physics2D.Raycast(OriginRaycast,Vector2.down,RaycastDistanceY);

        //Actualizamos la posicion de la camara con la del jugador
        if(R_left.collider == null && R_right.collider == null){
            tf.position = new Vector3(jugador_tf.position.x, tf.position.y, tf.position.z);
        }
        if(R_up.collider == null && R_down.collider == null){
            tf.position = new Vector3(tf.position.x, jugador_tf.position.y, tf.position.z);
        }
    }

    //Debug.DrawRay(OriginRaycast, Vector2.left * RaycastDistanceX, Color.red); //Pintar Raycast
    //Debug.DrawRay(OriginRaycast, Vector2.up * RaycastDistanceY, Color.green); //Pintar Raycast
    //Debug.DrawRay(OriginRaycast, Vector2.right * RaycastDistanceX, Color.yellow); //Pintar Raycast
    //Debug.DrawRay(OriginRaycast, Vector2.down * RaycastDistanceY, Color.blue); //Pintar Raycast
}

