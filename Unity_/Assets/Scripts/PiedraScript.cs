using UnityEngine;

public class PiedraScript : MonoBehaviour
{
    public int vidaPiedra;
    public bool normal;

    void Start(){
        vidaPiedra = normal ? 5 : 10;
    }

    public void DestruirPiedra(){
        //Dropear piedra
        GameObject piedra = Resources.Load<GameObject>("Prefabs/ObjetosInventario/Piedra");
        GameObject instancia = Instantiate(piedra, gameObject.transform.position, Quaternion.identity);

        instancia.GetComponent<Objeto>().cantidad = normal ? Random.Range(1, 4) : Random.Range(3, 8);

        Destroy(gameObject);
    }
}
