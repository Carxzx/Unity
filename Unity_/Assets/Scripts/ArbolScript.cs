using UnityEngine;
using UnityEngine.Tilemaps;

public class ArbolScript : MonoBehaviour
{
    public Tilemap tilemap;
    public int vidaArbol;

    void Start(){
        tilemap = GameObject.Find("TilemapArboles").GetComponent<Tilemap>();
        vidaArbol = 5;
    }

    public void DestruirArbol(){
        //Dropear madera
        GameObject madera = Resources.Load<GameObject>("Prefabs/ObjetosInventario/Madera");
        GameObject instancia = Instantiate(madera, gameObject.transform.position, Quaternion.identity);

        instancia.GetComponent<Objeto>().cantidad = Random.Range(2, 4);


        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        tilemap.SetTile(tilePos, null);

        //Destroy(gameObject);
    }
}
