using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

public class Clock : MonoBehaviour
{
    public TMP_Text time;
    Light2D luzGlobal;

    public int hour,minute;
    float deltaTime;

    const float timeSpeed = 0.5f; //Default = 1 para que un minuto en el juego sea un segundo IRL //0.5 creo que esta bien
    const int timeAddedPerCycle = 1;
    const int hoursPerDay = 18;
    float realTimePerDay;

    const int WakeUp = 8;
    const int GotoSleep = 2;

    public float normalizedTime;

    Color dia = Color.white;
    Color tarde = HexToColor("#EA8E3D"); //DE883C
    Color noche = HexToColor("#14163F");

    void Start(){
        luzGlobal = GameObject.Find("Global Light 2D").GetComponent<Light2D>();
        luzGlobal.color = dia;

        realTimePerDay = hoursPerDay * (timeSpeed / timeAddedPerCycle); //Tiempo real de un día en minutos

        hour = WakeUp;
        minute = 0;

        UpdateTime(hour,minute);
    }

    void Update(){
        if(!UI_Handler.ActiveUI){
            deltaTime += Time.deltaTime;

            if(deltaTime > timeSpeed){
                deltaTime = 0f;
                minute += timeAddedPerCycle;

                ManageTime();

                UpdateTime(hour,minute);
            }
            ManageGlobalLight();
        }
    }

    //Conversor de hora
    void ManageTime(){
        if(minute >= 60){
            minute = 0;
            hour += 1;

            if(hour >= 24){
                hour = 0;
            }
        }
    }

    //Updatear el tiempo del juego cada 5 minutos
    void UpdateTime(int hour, int minute){
        if(minute % 5 == 0){
            time.text = hour.ToString("00") + ':' + minute.ToString("00");
        }
    }

    void ManageGlobalLight(){
        const int WakeUp = 8;
        const int EmpiezaAtardecer = 14;
        const int EmpiezaOscurecer = 20;
        const int PararOscurecer = 24;

        if(hour >= WakeUp && hour < EmpiezaAtardecer){ //Desde las 8h a las 14h es totalmente de día
            normalizedTime = 0f;
        }else if(hour >= EmpiezaAtardecer && hour < EmpiezaOscurecer){ //Empieza a Atardecer a las 14h
            normalizedTime = (hour - EmpiezaAtardecer + (minute / 60f)) / (EmpiezaOscurecer - EmpiezaAtardecer);
            luzGlobal.color = Color.Lerp(dia, tarde, normalizedTime);
        }else if(hour >= EmpiezaOscurecer && hour < PararOscurecer){ //Empieza a Oscurecer a las 20h
            normalizedTime = (hour - EmpiezaOscurecer + (minute / 60f)) / (PararOscurecer - EmpiezaOscurecer);
            luzGlobal.color = Color.Lerp(tarde, noche, normalizedTime);
        }else{ //De las 0h a las 2h es totalmente de noche
            normalizedTime = 1f;
        }
    }

    public static Color HexToColor(string hex){
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
