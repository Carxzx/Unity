using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject ConectedDoor;
    public static bool TP;

    public static void PlayerTP(GameObject TransitionCircle, Door clickedDoor){
        Player.tf.position = clickedDoor.ConectedDoor.transform.GetChild(0).position;
        
        TP = true;
    }
}