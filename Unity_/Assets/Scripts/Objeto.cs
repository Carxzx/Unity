using UnityEngine;
using UnityEngine.UI;

public class Objeto : MonoBehaviour
{
    public int id, cantidad;
    public bool OnGround;

    InventoryData InventoryData;

    void Start(){
        InventoryData = FindFirstObjectByType<InventoryData>();
    }

    void Update(){
        if(OnGround && Player.InRange(transform.position,Player.AtractDistance) && !InventoryData.InventoryFull()){
            Atraer();
        }
    }

    void Atraer(){
        Vector3 posInic = gameObject.transform.position;
        Vector3 posFin = Player.tf.position;
        const float LerpSpeed = 0.007f;

        gameObject.transform.position = Vector3.Lerp(posInic, posFin, LerpSpeed);
    }
}

/*

ID:
    0 = None
    1 = Objeto cualquiera
    2 = Hacha
    3 = Pico



    */
