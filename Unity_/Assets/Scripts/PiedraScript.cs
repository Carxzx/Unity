using UnityEngine;
using UnityEngine.Tilemaps;

public class PiedraScript : MonoBehaviour
{
    public Tilemap tilemap;

    public int vidaPiedra;
    public bool normal;

    void Start(){
        tilemap = GameObject.Find("TilemapPiedras").GetComponent<Tilemap>();
        vidaPiedra = normal ? 5 : 10;
    }

    public void DestruirPiedra(){
        //Dropear piedra
        GameObject piedra = Resources.Load<GameObject>("Prefabs/ObjetosInventario/Piedra");
        GameObject instancia = Instantiate(piedra, gameObject.transform.position, Quaternion.identity);

        instancia.GetComponent<Objeto>().cantidad = normal ? Random.Range(1, 4) : Random.Range(3, 8);

        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        tilemap.SetTile(tilePos, null);
    }
}
