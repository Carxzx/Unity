using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FLor : MonoBehaviour
{
    public Tilemap tilemap;
    public int tipo;
    public AudioClip clip;

    void OnMouseOver()
    {
        // 1 = botón derecho del mouse
        if (Input.GetMouseButtonDown(1))
        {
            RightClickAction();
        }
    }

    void RightClickAction()
    {
        // Calcular distancia Manhattan (horizontal + vertical) en tiles
        float distanciaX = Mathf.Abs(transform.position.x - Player.tf.position.x);
        float distanciaY = Mathf.Abs(transform.position.y - Player.tf.position.y);

        if (distanciaX <= 2 && distanciaY <= 2)
        {
            DestruirFlor();
        }
        else
        {
            Debug.Log("Estás demasiado lejos para recoger la flor.");
        }
    }

    void Start(){
        tilemap = GameObject.Find("TilemapPiedras").GetComponent<Tilemap>();
    }

    public void DestruirFlor(){
        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        tilemap.SetTile(tilePos, null);

        InventoryData InventoryData = FindFirstObjectByType<InventoryData>();
        if(!InventoryData.InventoryFull()){
            bool bandera = false;
            int i = 0;

            while(i < InventoryData.numSlots && !bandera){
                if(InventoryData.vSlots[i].objeto.gameObject.GetComponent<Image>().sprite == GetComponent<SpriteRenderer>().sprite){
                    bandera = true;
                }
                i++;
            }
            i--;

            if(bandera){
                InventoryData.vSlots[i].objeto.cantidad += 1;
            }else{
                if(tipo == 1){
                    InventoryData.DarItem(1,"Prefabs/ObjetosInventario/Margarita");
                }else if(tipo == 2){
                    InventoryData.DarItem(1,"Prefabs/ObjetosInventario/Tulipan");
                }else{
                    InventoryData.DarItem(1,"Prefabs/ObjetosInventario/Plantita");
                }
            }

            PlayAndDestroy(clip);

            InventoryData.ObtenerSlots();
        }
        Destroy(gameObject);
    }

    void PlayAndDestroy(AudioClip clip)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = transform.position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = 1f;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }
}
