using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoLib
{
    public class Myo
    {
        public event EventHandler ConnectEvent;

        public event EventHandler DisConnectEvent;


        public void OnConnect()
        {
            if (ConnectEvent != null)
                ConnectEvent(this,null);
        }

        public void OnDisConnect()
        {
            if (DisConnectEvent != null)
                DisConnectEvent(this, null);
        }
    }
}
