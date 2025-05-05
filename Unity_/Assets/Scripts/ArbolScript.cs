using UnityEngine;

public class ArbolScript : MonoBehaviour
{
    public int vidaArbol;

    void Start(){
        vidaArbol = 5;
    }

    public void DestruirArbol(){
        //Dropear madera
        GameObject madera = Resources.Load<GameObject>("Prefabs/ObjetosInventario/Madera");
        GameObject instancia = Instantiate(madera, gameObject.transform.position, Quaternion.identity);

        instancia.GetComponent<Objeto>().cantidad = Random.Range(1, 4);

        Destroy(gameObject);
    }
}
