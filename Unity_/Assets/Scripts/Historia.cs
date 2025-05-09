using UnityEngine;
using System.Collections;

public class Historia : MonoBehaviour
{
    [System.Serializable]
    public class Dialogo{
        public string personaje;
        public string[] lineas;
        public bool finalizado_dialogo;
    }

    public static Dialogo CargarDialogo(string nombreArchivo){
        TextAsset json = Resources.Load<TextAsset>("Dialogs/" + nombreArchivo);
        if(json == null){
            Debug.LogError("No se encontró el archivo de diálogo: " + nombreArchivo);
            return null;
        }
        return JsonUtility.FromJson<Dialogo>(json.text);
    }

    UI_Handler UI_Handler;
    TextBox TextBox;

    void Start(){
        UI_Handler = FindFirstObjectByType<UI_Handler>();
        TextBox = UI_Handler.TextBox.GetComponent<TextBox>();
    }

    private IEnumerator MostrarDialogo(Dialogo dialogo){
        UI_Handler.ActivarUI(UI_Handler.TextBox);
        int i = 0;
        dialogo.finalizado_dialogo = false;
        while(i < dialogo.lineas.Length){
            TextBox.message = dialogo.lineas[i];
            TextBox.bandera = false;
            while(!TextBox.bandera){
                yield return null;
            }
            i++;
        }
        dialogo.finalizado_dialogo = true;
        UI_Handler.OcultarUI(UI_Handler.TextBox);
    }

    public void EncuentroMinotauro(){
        UI_Handler.EventOn();
        StartCoroutine(_EncuentroMinotauro());
    }

    private IEnumerator _EncuentroMinotauro(){
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Personajes/Minotauro_NPC");
        Vector2 pos = new Vector3(Player.tf.position.x, Player.tf.position.y-15,0);
        GameObject instancia = Instantiate(prefab,pos,Quaternion.identity);
        NPC_Minotauro minotauro = instancia.GetComponent<NPC_Minotauro>();

        Vector2 inicio = pos;
        Vector2 fin = new Vector2(inicio.x, Player.tf.position.y - 2);
        float distancePerFrame = 0.02f; //0.01f

        minotauro.Moverse(inicio,fin,distancePerFrame);

        //Esperar 3,5segundos
        yield return new WaitForSeconds(4f); //Esperar hasta que termine de moverse

        Dialogo dialogo = CargarDialogo("EncuentroMinotauro");
        StartCoroutine(MostrarDialogo(dialogo));

        while(!dialogo.finalizado_dialogo){
            yield return null;
        }


        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }
}
