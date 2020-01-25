/*
사용자의 움직임을 인식하여 걸음수가 올라가도록 함
*/
using UnityEngine;
using UnityEngine.UI;

public class StepCounter : MonoBehaviour // 걷는 것 카운트 하는 스크립트
{
    public Text StepCount;


    [Header("Pedometer")]
    public float lowLimit = 0.005F; // Level to fall to the low state. 
    public float highLimit = 0.1F; // Level to go to high state (and detect steps).
    private bool stateHigh = false; // Comparator state.

    public float filterHigh = 10.0F; // Noise filter control. Reduces frequencies above filterHigh private . 
    public float filterLow = 0.1F; // Average gravity filter control. Time constant about 1/filterLow.
    public float currentAcceleration = 0F; // Noise filter.
    float averageAcceleration = 0F;
    
    private int oldSteps;
    public float waitCounter = 0F;
    public float timeElapsedWalking = 0F;
    public float timeElapsedStandingStill = 0F;
    private bool startWaitCounter = false;

    void Awake()
    {
        averageAcceleration = Input.acceleration.magnitude; // Initialize average filter.
        oldSteps = Singleton.Instance.step;
    }



    void Update()
    {
        StepCount.text = Singleton.Instance.step + " / " + Singleton.Instance.range; // 싱글톤쓸때 이런식으로 쓰면 됨

        UpdateElapsedWalkingTime(); // Updates the time you spend while walking.
        WalkingCheck(); // Checks if you are walking or not.
    }

    void FixedUpdate()
    {
        // Filter Input.acceleration using Math.Lerp.
        currentAcceleration = Mathf.Lerp(currentAcceleration, Input.acceleration.magnitude, Time.deltaTime * filterHigh);
        averageAcceleration = Mathf.Lerp(averageAcceleration, Input.acceleration.magnitude, Time.deltaTime * filterLow);

        float delta = currentAcceleration - averageAcceleration; // Gets the acceleration pulses.

        if (!stateHigh)
        {
            // If the state is low.
            if (delta > highLimit)
            {
                // Only goes to high, if the Input is higher than the highLimit.
                stateHigh = true;
                Singleton.Instance.step++; // Counts the steps when the comparator goes to high.
            }
        }
        else
        {
            if (delta < lowLimit)
            {
                // Only goes to low, if the Input is lower than the lowLimit.
                stateHigh = false;
            }
        }
    }

    // Checks if you are walking or not.
    public void WalkingCheck()
    {
        if (Singleton.Instance.step != oldSteps)
        {
            startWaitCounter = true;
            waitCounter = 0F;
        }

        if (startWaitCounter)
        {
            waitCounter += Time.deltaTime;

            if (waitCounter != 0)
            {
                Singleton.Instance.isWalking = true;
            }
            if (waitCounter > 2.5)
            {
                waitCounter = 0F;
                startWaitCounter = false;
            }
        }
        else if (!startWaitCounter)
        {
            Singleton.Instance.isWalking = false;
        }
        oldSteps = Singleton.Instance.step;
    }

    // Updates the time you spend while walking.
    private void UpdateElapsedWalkingTime()
    {
        int secondsWalk = (int)(timeElapsedWalking % 60);
        int minutesWalk = (int)(timeElapsedWalking / 60) % 60;
        int hourWalk = (int)(timeElapsedWalking / 3600) % 24;

        int secondsStill = (int)(timeElapsedStandingStill % 60);
        int minutesStill = (int)(timeElapsedStandingStill / 60) % 60;
        int hoursStill = (int)(timeElapsedStandingStill / 3600) % 24;

        string timeElapsedWalkingString = string.Format("{0:0}:{1:00}:{2:00}", hourWalk, minutesWalk, secondsWalk);
        string timeElapsedStandingStillString = string.Format("{0:0}:{1:00}:{2:00}", hoursStill, minutesStill, secondsStill);

        if (Singleton.Instance.isWalking == true)
        {
            timeElapsedWalking += Time.deltaTime;
        }
        else if (Singleton.Instance.isWalking == false)
        {
            timeElapsedStandingStill += Time.deltaTime;
        }
    }

}