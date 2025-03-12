using UnityEngine;

public class Player : MonoBehaviour
{
    const float speed = 4f;

    public static Transform tf;
    public Rigidbody2D Rb;

    void Start(){
        
    }

    
    void Update(){
        float MovementX = Input.GetAxis("Horizontal");
        float MovementY = Input.GetAxis("Vertical");
        Rb.linearVelocity = new Vector2(speed * MovementX, Rb.linearVelocity.y);

        if(Input.GetKey(KeyCode.A)){
            Rb.linearVelocity = new Vector2(-speed, Rb.linearVelocity.y);
        }

        if(Input.GetKey(KeyCode.D)){
            Rb.linearVelocity = new Vector2(speed, Rb.linearVelocity.y);
        }

        if(Input.GetKey(KeyCode.W)){
            Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, speed);
        }

        if(Input.GetKey(KeyCode.S)){
            Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, -speed);
        }

        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)){
            Rb.linearVelocity = new Vector2(0f, Rb.linearVelocity.y);
        }

        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)){
            Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, 0f);
        }
    }
}
