using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Kinectronics
{
    public class Kinect
    {
        private KinectSensor kinectSensor = null;           //Create and initializes a KinectSensor object, used for setting the sensor active
        private GestureDetector gestureDetector = null;
        private BodyFrameReader bodyFrameReader = null;     //Create and initializes a BodyFrameReader object, used to get Body data from the sensor
        private Body currentTrackedBody = null;             //Create and initializes a Body object, used for tracking only one user at time
        private static int getControlBack = 0;              //Create and initializes an integer property, used for control the tries of getting back control of device if lost
        private static int timingFrames = 0;                //Create and initializes an integer property, used for stablisihing a specific response time for getting back control if lost
        private static int flag = 0;                        //Create and initializes an integer property, used for recognizing the hand state for the required time
        private ulong currentTrackingId = 0;                //Create and initializes a ulong property, used for linking a recongnized body with a specific tracking ID
        private static ulong matchingId = 0;                //Create and initializes a ulong property, used for saving a tracking ID and for comparing this in case of lost body
        public string gesture = null;

        //Method used for stablishing a connection with the sensor and opening the needed sensor data acquirers
        public void KinectConnect()
        {
            this.kinectSensor = KinectSensor.GetDefault();
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;
            this.kinectSensor.Open();
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();
            this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;
            this.gestureDetector = new GestureDetector(kinectSensor);
            this.gestureDetector.GestureDetected += Detector_GestureDetected;
        }

        //Method used for stoping the connection with the sensor and opening sensor data acquirers used
        public void KinnectDisconnect()
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.FrameArrived -= this.Reader_BodyFrameArrived;
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }
            if (this.kinectSensor != null)
            {
                this.kinectSensor.IsAvailableChanged -= this.Sensor_IsAvailableChanged;
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        //Method for cheking the sensor physical connection status
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            if (kinectSensor != null)
            {
                if (this.kinectSensor.IsAvailable != true)
                {
                    Console.WriteLine("Triyng to reach the Kinect Sensor");
                }
                else
                {
                    Console.WriteLine("Connection stablished");
                }
            }
        }

        //Method for measuring distance to a specific point
        private static double VectorLength(CameraSpacePoint point)
        {
            var result = Math.Pow(point.X, 2) + Math.Pow(point.Y, 2) + Math.Pow(point.Z, 2);

            result = Math.Sqrt(result);

            return result;
        }

        //Method that tracks all the available bodies at the environment and only considers the closest one
        private static Body FindClosestBody(BodyFrame bodyFrame)
        {
            Body result = null;
            double closestBodyDistance = double.MaxValue;

            Body[] bodies = new Body[bodyFrame.BodyCount];
            bodyFrame.GetAndRefreshBodyData(bodies);

            foreach (var body in bodies)
            {
                if (body.IsTracked)
                {
                    var currentLocation = body.Joints[JointType.SpineBase].Position;

                    var currentDistance = VectorLength(currentLocation);

                    if (result == null || currentDistance < closestBodyDistance)
                    {
                        result = body;
                        closestBodyDistance = currentDistance;
                    }
                }
            }

            return result;
        }

        private ulong CurrentTrackingId
        {
            get
            {
                return this.currentTrackingId;
            }

            set
            {
                this.currentTrackingId = value;
            }
        }

        //Method for linking an specific tracked body with the assigned tracking ID at the same frame
        //In case the saved matching ID is different from the current tracking ID, the method tries to get device control back
        private Body FindBodyWithTrackingId(BodyFrame bodyFrame, ulong trackingId)
        {
            Body result = null;

            Body[] bodies = new Body[bodyFrame.BodyCount];
            bodyFrame.GetAndRefreshBodyData(bodies);

            foreach (var body in bodies)
            {
                if (body.IsTracked)
                {
                    if (body.TrackingId == trackingId)
                    {
                        if (getControlBack == 0)
                        {
                            result = body;
                            break;
                        }
                        else
                        {
                            if (timingFrames < 331) //The getting connection back procedure listens to the getting back control gesture for 10 seconds
                            {
                                if (body.HandRightState == HandState.Closed)
                                {
                                    if (flag < 301)
                                    {
                                        if (flag < 300)
                                        {
                                            Console.WriteLine("{0:D}", flag + 1);
                                        }
                                        flag++; //In case it detects the getting control back gesture increases the recognition counter until gets back device control or loses it
                                    }
                                    if (flag == 300) //In case it achieves the required detection time, it fires the device control method again an restart the involved properties
                                    {
                                        Console.WriteLine("Control returned");
                                        matchingId = 0;
                                        flag = 0;
                                        getControlBack = 0;
                                        timingFrames = 0;
                                        break;
                                    }
                                }
                                timingFrames++;
                            }
                            if (timingFrames >= 331) //Otherwise, it fires the device security state method
                            {
                                Console.WriteLine("Time Elapsed, device returning to security state");
                                matchingId = 0;
                                flag = 0;
                                getControlBack = 0;
                                timingFrames = 0;
                                KinectEventArgs args = new KinectEventArgs();
                                args.KinectEvent = KinectEvent.UserControlLost;
                                OnKinectEventTriggered(args);
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        //Method that reads the arriving frames, and calls the already described frames for the said reasons
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var frameReference = e.FrameReference;
            bool dataReceived = false;

            using (var frame = frameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    // We might miss the chance to acquire the frame, it will be null if it's missed
                    return;
                }

                if (this.currentTrackedBody != null)
                {
                    this.currentTrackedBody = FindBodyWithTrackingId(frame, this.CurrentTrackingId);

                    if (this.currentTrackedBody != null)
                    {
                        return;
                    }
                }

                Body selectedBody = FindClosestBody(frame);

                if (selectedBody == null)
                {
                    return;
                }

                if (getControlBack != 1)
                {
                    Console.WriteLine("Body Tracked");
                    Console.WriteLine("Trying to connect to device");
                }

                this.currentTrackedBody = selectedBody;
                this.CurrentTrackingId = selectedBody.TrackingId;


                if (matchingId == 0) //In case is the first time a user is tracked, it saves its tracking ID for possible eventualities
                {
                    matchingId = selectedBody.TrackingId;
                    if (getControlBack != 1)
                    {
                        dataReceived = true;
                        Console.WriteLine("Tracking ID: {0:N}", selectedBody.TrackingId);
                    }
                }

                if (matchingId != selectedBody.TrackingId) //In case the previoulsy saved ID does not matches the current tracked ID, it starts the trying to get control back procedure
                {
                    matchingId = selectedBody.TrackingId;
                    Console.WriteLine("The tracking ID changed");
                    Console.WriteLine("New tracking ID: {0:N} ", selectedBody.TrackingId);
                    Console.WriteLine("If you want to control the device close your hand for 10 seconds");
                    getControlBack = 1;
                }
            }
            if (dataReceived)
            {
                if (this.currentTrackedBody != null)
                {
                    ulong trackingId = this.currentTrackedBody.TrackingId;

                    // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                    if (trackingId != this.gestureDetector.TrackingId)
                    {
                        this.gestureDetector.TrackingId = trackingId;

                        // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                        // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                        this.gestureDetector.IsPaused = trackingId == 0;                       
                    }
                }
            }
        }

        protected virtual void OnKinectEventTriggered(KinectEventArgs e)
        {
            EventHandler<KinectEventArgs> handler = KinectEventTriggered;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<KinectEventArgs> KinectEventTriggered;

        private void Detector_GestureDetected(object sender, ChangedEventArgs e)
        {
            gesture = e.gestureName;         
        }

        public string getGestureName()
        {
            return gesture;
        }
    }
}
