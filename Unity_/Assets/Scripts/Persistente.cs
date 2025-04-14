using UnityEngine;

public class Persistente : MonoBehaviour
{
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
}
