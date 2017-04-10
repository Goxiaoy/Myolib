using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace MyoLib.Bluetooth
{

    public struct MyoGattDeviceService
    {
        public DeviceInformation device;
        public GattDeviceService service;
        public bool isInitialize;
    }

    /// <summary>
    /// Myo Connect Update Handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="isConnected"></param>
    public delegate void DeviceConnectUpdateHandler(MyoHw sender,bool isConnected);
    /// <summary>
    /// Myo Class(Hardware)
    /// </summary>
    public class MyoHw
    {
        /// <summary>
        /// Myo id
        /// </summary>
        private DeviceInformation m_Controldevice;

        /// <summary>
        /// Myo
        /// </summary>
        private Myo m_myo;

        /// <summary>
        /// Gatt Control Service
        /// </summary>
        public MyoGattDeviceService m_ControlService;
        /// <summary>
        /// Gatt ImuData Service
        /// </summary>
        public MyoGattDeviceService m_ImuDataService;
        /// <summary>
        /// Gatt Classifier Service
        /// </summary>
        public MyoGattDeviceService m_ClassifierService;
        /// <summary>
        /// Gatt EmgData Service
        /// </summary>
        public MyoGattDeviceService m_EmgDataService;

        /// <summary>
        /// Gatt Battery Service
        /// </summary>
        public MyoGattDeviceService m_BatteryService;
        /// <summary>
        /// 
        /// </summary>
        public string deviceContainerId;


        /// <summary>
        /// Connect update event
        /// </summary>
        public event DeviceConnectUpdateHandler DeviceConnectionUpdated;



        /// <summary>
        /// Myo id
        /// </summary>
        public DeviceInformation Device
        {
            get { return m_Controldevice; }      
        }

        public Myo Myo
        {
            get { return m_myo; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        public MyoHw(DeviceInformation device)
        {
            m_Controldevice = device;
            m_ControlService.device = device;

            deviceContainerId = "{" + device.Properties["System.Devices.ContainerId"] + "}";

            m_myo = new Myo();

           
        }

        public void OnConnectUpdate(bool isConnect)
        {
            if(isConnect)
                m_myo.OnConnect();
            else
            {
                m_myo.OnDisConnect();
            }

            if (DeviceConnectionUpdated != null)
                DeviceConnectionUpdated(this,isConnect);
        }

    }
}
