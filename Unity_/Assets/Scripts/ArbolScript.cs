using UnityEngine;

public class ArbolScript : MonoBehaviour
{
    public int vidaArbol;

    void Start(){
        vidaArbol = 10;
    }

    public void DestruirArbol(){
        //Dropear madera
        GameObject madera = Resources.Load<GameObject>("Prefabs/ObjetosInventario/Madera");
        Instantiate(madera, gameObject.transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
