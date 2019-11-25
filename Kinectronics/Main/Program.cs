using Kinectronics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    class Program
    {
        private static string connectionString = "WiFi";
        private static Bulldozer vehicle = null;
        private static Kinect kinect = null;

        static void Main(string[] args)
        {
            kinect = new Kinect();
            kinect.KinectEventTriggered += c_KinectEventTriggered;
            kinect.KinectConnect();
            vehicle = new Bulldozer(connectionString);
            vehicle.StablishConnection();

            while (true)
            {
                stringGestureDetected(kinect.getGestureName());
            }
        }

        private static void c_KinectEventTriggered(object sender, KinectEventArgs e)
        {
            if (e.KinectEvent == KinectEvent.UserControlLost)
            {
                Console.WriteLine("Event Triggered");
                kinect.KinnectDisconnect();
                //vehicle.SecuritySpeed();
            }
        }

        private static void stringGestureDetected(string gestureName)
        {
            switch (gestureName)
            {
                case "Arms45DownPosition":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    vehicle.BladeDown();
                    kinect.gesture = null;
                    break;
                /*case "Arms45UpPosition":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    kinect.gesture = "No Gesture";
                    //vehicle.Stop();
                    break;*/
                case "ArmsFrontPosition_Left":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    vehicle.MoveForward();
                    kinect.gesture = null;
                    //vehicle.Stop();
                    break;
                case "ArmsFrontPosition_Right":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    vehicle.BladeUp();
                    kinect.gesture = null;
                    //vehicle.Stop();
                    break;
                case "ArmsHRectanglePosition_Left":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    kinect.gesture = null;
                    //vehicle.Stop();
                    break;
                case "ArmsHRectanglePosition_Right":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    vehicle.MoveBackward();
                    kinect.gesture = null;
                    //vehicle.Stop();
                    break;
                /*case "ArmsRectanglePosition_Left":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    //vehicle.Stop();
                    break;
                case "ArmsRectanglePosition_Right":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    //vehicle.Stop();
                    break;*/
                case "ArmsSidePosition_Left":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    vehicle.TurnLeft();
                    kinect.gesture = null;
                    //vehicle.Stop();
                    break;
                case "ArmsSidePosition_Right":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    vehicle.TurnRight();
                    kinect.gesture = null;
                    //vehicle.Stop();
                    break;
                /*case "ArmsSquarePosition_Left":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    //vehicle.Stop();
                    break;
                case "ArmsSquarePosition_Right":
                    Console.WriteLine("Gesture: {0}", gestureName);
                    //vehicle.Stop();
                    break;*/
                default:
                    //vehicle.Stop();
                    kinect.gesture = null;
                    //vehicle.Stop();
                    break;
            }
        }
    }
}
