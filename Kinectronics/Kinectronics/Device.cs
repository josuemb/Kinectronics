using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectronics
{
    public class Device
    {

        private int maxReachDistance = 0;
        private string connectionString = null;

        public Device(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public virtual void StablishConnection()
        {
            Console.WriteLine("Connection Stablished");
        }

        public virtual void StopConnection()
        {
            Console.WriteLine("Device Disconnected");
        }
    }
}
