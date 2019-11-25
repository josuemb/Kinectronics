using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoBrick.EV3;

namespace Kinectronics
{
    public class EV3GroundVehicle : GroundVehicle
    {
        protected Brick<Sensor, Sensor, Sensor, Sensor> ev3 = null;
        protected Motor[] motors = null;

        public EV3GroundVehicle(string connectionString) : base(connectionString)
        {
            ev3 = new Brick<Sensor, Sensor, Sensor, Sensor>(connectionString);
        }

        public override void DecreaseSpeed()
        {
            base.DecreaseSpeed();
        }

        public override void IncreaseSpeed()
        {
            base.IncreaseSpeed();
        }

        public override void MoveForward()
        {
            base.MoveForward();
        }

        public override void Return()
        {
            base.Return();
        }

        public override void SecuritySpeed()
        {
            base.SecuritySpeed();
        }

        public override void StablishConnection()
        {
            base.StablishConnection();
            ev3.Connection.Open();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void StopConnection()
        {
            base.StopConnection();
            ev3.Connection.Close();
        }
    }
}
