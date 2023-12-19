using System;
using System.IO.Ports;
using UnityEngine;

public class SerialHandler : MonoBehaviour
{
    public static SerialHandler instance;

    public SerialPort _serial;

    // Common default serial device on a Windows machine
    [SerializeField] private string serialPort = "COM3";
    [SerializeField] private int baudrate = 115200;

    
    //[SerializeField] private Component river;
    private Rigidbody2D _riverRigidbody2D;
    private SpriteRenderer _riverSprite;

    [HideInInspector] public int xValueJoystick = 0, yValueJoystick = 0;
    [HideInInspector] public float xValueAcc = 0, yValueAcc = 0, zValueAcc = 0;
    [HideInInspector] public int pressJoystick = 0;
    [HideInInspector] public int lastPressJoystick = 0;

    // Start is called before the first frame update
    void Start()
    {
        _serial = new SerialPort(serialPort,baudrate);
        // Guarantee that the newline is common across environments.
        _serial.NewLine = "\n";
        // Once configured, the serial communication must be opened just like a file : the OS handles the communication.
        _serial.Open();
        instance = this;
        
        //_riverRigidbody2D = river.GetComponentInParent<Rigidbody2D>();
        //_riverSprite = river.GetComponentInParent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lastPressJoystick = pressJoystick;
        // Prevent blocking if no message is available as we are not doing anything else
        // Alternative solutions : set a timeout, read messages in another thread, coroutines, futures...
        if (_serial.BytesToRead <= 0) return;
        
        // Trim leading and trailing whitespaces, makes it easier to handle different line endings.
        // Arduino uses \r\n by default with `.println()`.
        var message = _serial.ReadLine().Trim();

        if (message == "dry")
        {
            _riverRigidbody2D.simulated = false;
            _riverSprite.color = new Color32(146, 108, 77, 255);
        }
        else if (message == "wet")
        {
            _riverRigidbody2D.simulated = true;
            _riverSprite.color = new Color32(16, 107, 255, 255);
        }
        else if(message.Length > 0)
        {

            if (message[0] == 'x')//Joystick x
            {
                xValueJoystick = int.Parse(message.Substring(1));
            }
            else if (message[0] == 'y')//Joystick y
            {
                yValueJoystick = int.Parse(message.Substring(1));
            }
            else if (message[0] == 'X')//Accélération X
            {
                xValueAcc = float.Parse(message.Substring(1).Replace(".",","));
            }
            else if (message[0] == 'Y')//Accélération Y
            {
                yValueAcc = float.Parse(message.Substring(1).Replace(".", ","));
            }
            else if (message[0] == 'Z')//Accélération Z
            {
                zValueAcc = float.Parse(message.Substring(1).Replace(".", ","));
            }
            else if (message[0] == 'b')//Accélération Z
            {
                pressJoystick = int.Parse(message.Substring(1));
            }

            //Debug.Log("xValueJoystick " + xValueJoystick + " yValueJoystick " + yValueJoystick);
            //Debug.Log("xValueAcc " + xValueAcc + " yValueAcc " + yValueAcc + " zValueAcc " + zValueAcc);

        }
        
    }

    public void SetLed(bool newState)
    {
        _serial.WriteLine(newState ? "LED ON" : "LED OFF");
        Debug.Log(newState ? "LED ON" : "LED OFF");
    }

    public void SendMessageToArduino(string msg)
    {
        _serial.WriteLine(msg);
    }

    private void OnDestroy()
    {
        _serial.Close();
    }
}
