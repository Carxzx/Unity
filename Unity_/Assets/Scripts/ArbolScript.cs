using UnityEngine;

public class ArbolScript : MonoBehaviour
{
    public int vidaArbol;

    void Start(){
        vidaArbol = 10;
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Hacha")){
            vidaArbol--;
            if(vidaArbol <= 0){
                DestruirArbol();
            }
        }
    }

    void DestruirArbol(){
        //Dropear madera
        GameObject madera = Resources.Load<GameObject>("Prefabs/ObjetosInventario/Madera");
        Instantiate(madera, gameObject.transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
