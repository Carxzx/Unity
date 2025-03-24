using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    public TMP_Text time;

    int hour,minute;

    public bool isWorking;
    float timeSpeed;
    float tiempo;

    void Start(){
        isWorking = false;
        hour = 8;
        minute = 0;

        UpdateTime(hour,minute);
    }

    void Update(){
        if(!UI_Handler.ActiveUI){
            tiempo += Time.deltaTime;

            if(tiempo > 5){
                tiempo = 0f;
                minute += 5;

                ManageTime();

                UpdateTime(hour,minute);
            }
        }
    }

    void ManageTime(){
        if(minute >= 60){
            minute = 0;
            hour += 1;

            if(hour >= 24){
                hour = 0;
            }
        }
    }

    void UpdateTime(int hour, int minute){
        time.text = hour.ToString("00") + ':' + minute.ToString("00");
    }
}
