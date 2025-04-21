using UnityEngine;
using UnityEngine.UI;

public class Objeto : MonoBehaviour
{
    public int id, cantidad;
    public bool OnGround;

    void Start(){
        if(OnGround){
            ActualizarSprite();
        }else{
            if(transform.parent.name != "objetoEnMano"){
                Slot slot = transform.parent.GetComponent<Slot>();
                slot.ActualizarSprite(GetComponent<Objeto>());
            }
        }
    }

    void Update(){
        InventoryData InventoryData = FindFirstObjectByType<InventoryData>();
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

    void ActualizarSprite(){
        SpriteRenderer Sprite = gameObject.GetComponent<SpriteRenderer>();
        SpriteManager SpriteManager = FindFirstObjectByType<SpriteManager>();

        Sprite.sprite = SpriteManager.vSprite[id];
        Sprite.enabled = id != 0;
    }

    public void Hacha(){
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Hacha");
        Instantiate(prefab, Player.tf.position, Quaternion.identity);
    }
}

/*

ID:
    1 = Hacha
    2 = Madera



    */
