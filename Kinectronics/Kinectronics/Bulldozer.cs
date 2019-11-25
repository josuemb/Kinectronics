using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoBrick.EV3;

namespace Kinectronics
{
    public class Bulldozer : EV3GroundVehicle
    {
        private sbyte turnspeed, movespeed, bladespeed;

        public Bulldozer(string connectionString) : base(connectionString)
        {
            motors = new Motor[] { ev3.MotorC };
        }

        public override void DecreaseSpeed()
        {
            base.DecreaseSpeed();
        }

        public override void IncreaseSpeed()
        {
            base.IncreaseSpeed();
        }

        public override void MoveBackward()
        {
            base.MoveBackward();
            Console.WriteLine("Buldoser move backward\n");
            turnspeed = 10;
            ev3.MotorC.On(turnspeed, 35, true);
        }

        public override void MoveForward()
        {
            base.MoveForward();
            Console.WriteLine("Buldoser move forward\n");
            movespeed = -10;
            ev3.MotorC.On(movespeed, 35, true);
            //motors[0].MoveTo(30, 0, false);
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
        }

        public override void Stop()
        {
            base.Stop();
            ev3.MotorA.Brake();
            ev3.MotorB.Brake();
            ev3.MotorC.Brake();
            //motors[0].Off();
        }

        public override void StopConnection()
        {
            base.StopConnection();
        }

        public override void TurnLeft()
        {
            base.TurnLeft();
            Console.WriteLine("Buldoser turn left\n");
            turnspeed = 1;
            ev3.MotorB.On(turnspeed, 5, true);
        }

        public override void TurnRight()
        {
            base.TurnRight();
            Console.WriteLine("Buldoser turn right\n");
            turnspeed = -1;
            ev3.MotorB.On(turnspeed, 5, true);
        }

        public void BladeUp()
        {
            Console.WriteLine("Blade up\n");
            bladespeed = 1;
            ev3.MotorA.On(bladespeed, 2, true);
        }

        public void BladeDown()
        {
            Console.WriteLine("Blade down\n");
            bladespeed = -1;
            ev3.MotorA.On(bladespeed, 2, true);
        }
    }
}
