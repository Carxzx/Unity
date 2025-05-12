using UnityEngine;
using System.Collections;
using TMPro;

public class Historia : MonoBehaviour
{
    [System.Serializable]
    public class Dialogo{
        public int id;
        public string personaje;
        public string[] lineas;
        public bool finalizado_dialogo;
    }

    [System.Serializable]
    public class BloquesDialogo{
        public Dialogo[] dialogo;
    }

    public BloquesDialogo CargarDialogo(string nombreArchivo){
        TextAsset json = Resources.Load<TextAsset>("Dialogs/" + nombreArchivo);
        if(json == null){
            Debug.LogError("No se encontró el archivo de diálogo: " + nombreArchivo);
            return null;
        }

        BloquesDialogo bloques = JsonUtility.FromJson<BloquesDialogo>(json.text);

        return bloques;
    }

    UI_Handler UI_Handler;
    TextBox TextBox;
    public TMP_InputField inputField;
    public string nombreJugador;

    void Start(){
        UI_Handler = FindFirstObjectByType<UI_Handler>();
        TextBox = UI_Handler.TextBox.GetComponent<TextBox>();
    }

    private IEnumerator MostrarDialogo(Dialogo dialogo){
        TextBox.gameObject.SetActive(true);
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
        TextBox.gameObject.SetActive(false);
    }

    void ActualizarNombreJSON(BloquesDialogo bloques){
        if (PlayerPrefs.HasKey("nombreJugador")){
            string nombreJugador = PlayerPrefs.GetString("nombreJugador");
            inputField.text = nombreJugador;
            foreach (Dialogo d in bloques.dialogo){
                for (int i = 0; i < d.lineas.Length; i++){
                    d.lineas[i] = d.lineas[i].Replace("{nombre}", nombreJugador);
                }
            }
        }
    }

    public void _Prueba(){
        StartCoroutine(Prueba());
    }

    private IEnumerator Prueba(){
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Personajes/Minotauro_NPC");
        Vector2 pos = new Vector3(Player.tf.position.x, Player.tf.position.y-15,0);
        GameObject instancia = Instantiate(prefab,pos,Quaternion.identity);
        NPC_Minotauro minotauro = instancia.GetComponent<NPC_Minotauro>();

        Vector2 inicio = pos;
        Vector2 fin = new Vector2(inicio.x, Player.tf.position.y - 2);
        float distancePerFrame = 0.01f; //0.02f

        minotauro.Moverse(inicio,fin,distancePerFrame);

        //Esperar 3,5segundos
        yield return new WaitForSeconds(4f); //Esperar hasta que termine de moverse

        BloquesDialogo bloques = CargarDialogo("Prueba");
        int numDialogo = 0;
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public void _BosquePrimeraInteraccion(){
        StartCoroutine(BosquePrimeraInteraccion());
    }

    private IEnumerator BosquePrimeraInteraccion(){
        NPC_Minotauro minotauro = GameObject.Find("MinotauroBosque").GetComponent<NPC_Minotauro>();

        //Exclamacion del minotauro por 2 segundos
        StartCoroutine(Icons.MostrarIcono(minotauro.gameObject,"exclamacion",1f));
        yield return new WaitForSeconds(1f);

        //Girarse hacia arriba
        minotauro.Girarse(Vector2.up);

        //Breve pausa
        yield return new WaitForSeconds(0.5f);

        Vector2 inicio = minotauro.transform.position;
        Vector2 fin = new Vector2(Player.tf.position.x, inicio.y);
        float distancePerFrame = 0.01f; //0.02f
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }

        inicio = minotauro.transform.position;
        fin = new Vector2(inicio.x, Player.tf.position.y-2.5f);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }

        //Breve pausa tras dejar de moverse
        yield return new WaitForSeconds(0.2f);

        //Empieza el dialogo
        BloquesDialogo bloques = CargarDialogo("BosquePrimeraInteraccion");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        //Breve pausa
        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        //duda del minotauro por 1 segundo
        StartCoroutine(Icons.MostrarIcono(minotauro.gameObject,"duda",1f));
        yield return new WaitForSeconds(1.5f);

        //Dialogo id = 2
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;


        //
        //METER ELECCION MULTIPLE
        //


        //Dialogo id = 3
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        //Breve pausa
        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 4
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;


        //Dialogo id = 5
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;


        //Guardar nombre del jugador
        PlayerPrefs.DeleteAll(); ///////////////////
        inputField.gameObject.SetActive(true);

        while(!Input.GetKeyDown(KeyCode.Return) || inputField.text.Length == 0){
            yield return null;
        }

        nombreJugador = inputField.text;
        Debug.Log("Enter detectado con nombre: " + inputField.text);

        PlayerPrefs.SetString("nombreJugador", nombreJugador);
        PlayerPrefs.Save(); // fuerza el guardado
        inputField.gameObject.SetActive(false);

        ActualizarNombreJSON(bloques);
        PlayerPrefs.DeleteAll(); //////////////

        //Dialogo id = 6
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        TextBox.MoverTextBox();

        //Se empieza a mover de nuevo
        inicio = minotauro.transform.position;
        fin = new Vector2(-999.5f, inicio.y);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }

        inicio = minotauro.transform.position;
        fin = new Vector2(inicio.x, inicio.y-3f);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }

        //Breve pausa tras dejar de moverse
        yield return new WaitForSeconds(0.2f);


        //Dialogo id = 7
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return null;

        //Se gira hacia arriba
        minotauro.Girarse(Vector2.up);

        //Breve pausa
        yield return new WaitForSeconds(0.2f);

        //Dialogo id = 8
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        //Empezar a moverse de nuevo
        inicio = minotauro.transform.position;
        fin = new Vector2(inicio.x, 986f);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }


        inicio = minotauro.transform.position;
        fin = new Vector2(inicio.x+1f, inicio.y);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }
        

        inicio = minotauro.transform.position;
        fin = new Vector2(inicio.x, 984f);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }

        Destroy(minotauro.gameObject);

        TextBox.MoverTextBox();
        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }
}
