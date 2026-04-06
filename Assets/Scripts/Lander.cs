using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    private const float GRAVITY_NORMAL = 0.7f;

    public event EventHandler OnUpForce; 
    public event EventHandler OnRightForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickUp;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
         public State state;
    }
    public event EventHandler<OnLandedEventArgs> OnLanded;
        
    public class OnLandedEventArgs : EventArgs
    {
        public LandingType landingType;
        public int score;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier; 

    }

    private float fuelAmmount;
    private float fuelAmmountMax = 10f;
    private State state;
    public static Lander Instance;

    public enum LandingType
    {
        Success,
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding,
    }


    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver,
    }


    private Rigidbody2D landerRigidbody2D;

    private void Awake()
    {
       landerRigidbody2D = GetComponent<Rigidbody2D>();
        Instance = this;

        fuelAmmount = fuelAmmountMax;

        state = State.WaitingToStart;

        landerRigidbody2D = GetComponent<Rigidbody2D>();

        landerRigidbody2D.gravityScale = 0f;


    }


    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        switch (state)
        {
            default:
                case State.WaitingToStart:

                if (Keyboard.current.upArrowKey.isPressed ||
                    Keyboard.current.leftArrowKey.isPressed ||
                    Keyboard.current.rightArrowKey.isPressed)
                {
                    landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
                    setState(State.Normal);
                }
                break;

                case State.Normal:
                if (fuelAmmount <= 0f)
                {
                    return;
                }

                if (Keyboard.current.upArrowKey.isPressed ||
                    Keyboard.current.leftArrowKey.isPressed ||
                    Keyboard.current.rightArrowKey.isPressed)
                {
                    ConsumeFuel();
                }

                if (Keyboard.current.upArrowKey.isPressed)
                {
                    float force = 700f;
                    landerRigidbody2D.AddForce(force * transform.up * Time.deltaTime);
                    OnUpForce?.Invoke(this, EventArgs.Empty);
                }
                if (Keyboard.current.rightArrowKey.isPressed)
                {
                    float turnSpeed = -100f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
                    OnRightForce?.Invoke(this, EventArgs.Empty);
                }
                if (Keyboard.current.leftArrowKey.isPressed)
                {
                    float turnSpeed = +100f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
                    OnLeftForce?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }

      
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Crash on Terrain!");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.WrongLandingArea,
                dotVector = 0f,
                landingSpeed = 0,
                scoreMultiplier = 0,
                score = 0,
            });
            setState(State.GameOver);   
            return;

        }
        float softLandindVelocityMagnitude = 4f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandindVelocityMagnitude)
        {
            Debug.Log("Landed too hard!");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooFastLanding,
                dotVector = 0f,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0,

            });
            setState(State.GameOver);
            return;
        }

       
        float dotVector = Vector2.Dot(Vector2.up,transform.up);
        float minDotVector = .90f;
        if (dotVector < minDotVector)
        {
            //Land on a too steep angle 
            Debug.Log("Land on a too steep angle");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooSteepAngle,
                dotVector = dotVector,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0,

            });
            setState(State.GameOver);
            return ;
        }


        Debug.Log("Landing Successfully ");

        float maxScoreLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreLandingAngle ;


        float maxScoreAmountLandingSpeed = 100;
        float landingSpeedScore = (softLandindVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;
            

        Debug.Log("landingAngleScore: " + landingAngleScore);
        Debug.Log("landingSpeedScore: " + landingSpeedScore);

        int score = Mathf.RoundToInt((landingSpeedScore + landingAngleScore) * landingPad.getScoreMultiplier());

        Debug.Log("Score: " + score);

        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            landingType = LandingType.Success,
            dotVector = dotVector,
            landingSpeed = relativeVelocityMagnitude,
            scoreMultiplier = landingPad.getScoreMultiplier(),
            score = score,
        });

        setState(State.GameOver);


    }



    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.TryGetComponent(out FuelPickUp fuelPickUp))
        {
            float addFuelAmount = 10f;
            fuelAmmount += addFuelAmount;
            if(fuelAmmount > fuelAmmountMax)
            {
                fuelAmmount = fuelAmmountMax;
            }
            fuelPickUp.DestroySelf();
        }

        if (collider2D.gameObject.TryGetComponent(out CoinPickUp coinPickUp))
        {
            OnCoinPickUp?.Invoke(this, EventArgs.Empty);
            coinPickUp.DestroySelf();
        }

    }

    private void setState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        { state = state });
    }

    private void ConsumeFuel()
    {
        float fuelConsumptionAmount = 1f;
        fuelAmmount -= fuelConsumptionAmount * Time.deltaTime;
    }

    public float GetFuel()
    {
        return fuelAmmount;

    }

    public float GetFuelAmmountNormalize()
    {
        return fuelAmmount / fuelAmmountMax;
    }

    public float GetSpeedX()
    {
        return landerRigidbody2D.linearVelocityX;
    }
    public float GetSpeedY()
    {
        return landerRigidbody2D.linearVelocityY;
    }
}

    

