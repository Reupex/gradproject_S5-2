using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private static CarUserControl _instance;
        public static CarUserControl instance
        {
            get { return _instance; }
        }
        private CarController m_Car; // the car controller we want to use
        float h = 0;
        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            _instance = GetComponent<CarUserControl>();
        }

        public void handle(float handle)
        {
            h = handle;
        }
        private void FixedUpdate()
        {
            // pass the input to the car!
            float v = 1;
            //float h = CrossPlatformInputManager.GetAxis("Horizontal");
            
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
