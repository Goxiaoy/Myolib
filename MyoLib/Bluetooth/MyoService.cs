using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;
using MyoLib;
using System.Threading.Tasks;

namespace MyoLib.Bluetooth
{

    public delegate void ServiceFunction();

    /// <summary>
    /// Main Service of Myo, Singleton
    /// </summary>
    public class MyoService
    {
        /// <summary>
        /// MYO main service uuid
        /// </summary>
        static readonly Guid MYO_MAIN_UUID = new Guid("D5060001-A904-DEB9-4748-2C7F4A124842");

        /// <summary>
        /// MYO main service uuid
        /// </summary>
        static readonly Guid MYO_IMU_UUID = new Guid("D5060002-A904-DEB9-4748-2C7F4A124842");

        /// <summary>
        /// MYO main service uuid
        /// </summary>
        static readonly Guid MYO_CLASSIFIER_UUID = new Guid("D5060003-A904-DEB9-4748-2C7F4A124842");

        /// <summary>
        /// MYO main service uuid
        /// </summary>
        static readonly Guid MYO_EMG_UUID = new Guid("D5060005-A904-DEB9-4748-2C7F4A124842");

        /// <summary>
        /// MYO main service uuid
        /// </summary>
        static readonly Guid MYO_BATTERY_UUID = new Guid("D506180f-A904-DEB9-4748-2C7F4A124842");
        /// <summary>
        /// instance
        /// </summary>
        private static MyoService instance = new MyoService();
        /// <summary>
        /// All paired Devices Information Collection
        /// </summary>
        private DeviceInformationCollection devicesCollection;
        /// <summary>
        /// Paired myos
        /// </summary>
        private List<MyoHw> pairedMyos=new List<MyoHw>();
        /// <summary>
        /// Connected myos
        /// </summary>
        private List<MyoHw> connectedMyos=new List<MyoHw>();

        /// <summary>
        /// Connect Watcher
        /// </summary>
        private PnpObjectWatcher watcher;


        private event ServiceFunction InitEvent;

        private event ServiceFunction ConnnectEvent;

        /// <summary>
        /// MyoService
        /// </summary>
        public static MyoService Instance
        {
            get { return instance; }
        }
        /// <summary>
        /// Constructor, Init
        /// </summary>
        private MyoService()
        {
            InitEvent += Init;
            ConnnectEvent += ConnectDevice;
        }

        public List<Myo> GetPairedMyo()
        {
            List<Myo> t_myos=new List<Myo>();
            foreach (var myoHw in pairedMyos)
            {
                t_myos.Add(myoHw.Myo);
            }
            return t_myos;
        }


        public List<Myo> GetConnectedMyo()
        {
            List<Myo> t_myos = new List<Myo>();
            foreach (var myoHw in connectedMyos)
            {
                t_myos.Add(myoHw.Myo);
            }
            return t_myos;
        }

        public void OnInit()
        {
            if (InitEvent != null)
                InitEvent();
        }

        public void OnConnect()
        {
            if (ConnnectEvent != null)
                ConnnectEvent();
        }

        /// <summary>
        /// Search paired device 
        /// </summary>
        private async void Init()
        {
            try
            {
                //Clear MyoList
                pairedMyos = new List<MyoHw>();
                connectedMyos = new List<MyoHw>();
                //Search paired Myo
                devicesCollection = await DeviceInformation.FindAllAsync(
                    GattDeviceService.GetDeviceSelectorFromUuid(MYO_MAIN_UUID),
                    new string[] { "System.Devices.ContainerId" });
                if (devicesCollection.Count > 0)
                {
                    StartDeviceConnectionWatcher();
                    foreach (var device in devicesCollection)
                    {                      
                        pairedMyos.Add(new MyoHw(device));                  
                    }
                }
                else
                {
                    throw new Exception("No Myo Found, Please Pair Your Myo First");
                }
            }
            catch (Exception e)
            {              
                throw(new Exception("Search Fail!"+e.Message));
            }       

        }
        /// <summary>
        /// Try to connect each Paired Device
        /// </summary>
        /// <returns></returns>
        private async void ConnectDevice()
        {
            try
            {
                foreach (var myoHw in pairedMyos)
                {
                    await ConnectDevice(myoHw);
                }
            }
            catch (Exception e)
            {               
                throw e;
            }
        }

        /// <summary>
        /// Connect Myo
        /// </summary>
        /// <param name="myo"></param>
        /// <returns></returns>
        private async Task ConnectDevice(MyoHw myo)
        {
            try
            {
                //Continue to Connect to other service once connect success to ControlService
                myo.DeviceConnectionUpdated += ContinueConnect;

                myo.m_ControlService.service = await GattDeviceService.FromIdAsync(myo.Device.Id);

                if (myo.m_ControlService.service != null)
                {
                    myo.m_ControlService.isInitialize = true;

                    //await ConfigureServiceForNotificationsAsync();
                }
                else
                {
                    throw new Exception("Access to the device is denied, because the application was not granted access, " +
                        "or the device is currently in use by another application.");
                }
            }
            catch (Exception e)
            {                
                throw new Exception("Connect fail"+ myo.Device.Id+"\n"+ e.Message);
            }
           
        }

        private async void ContinueConnect(MyoHw myo, bool isConnect)
        {
            devicesCollection = await DeviceInformation.FindAllAsync(
                GattDeviceService.GetDeviceSelectorFromUuid(MYO_EMG_UUID),
                new string[] {"System.Devices.ContainerId"});

            devicesCollection = await DeviceInformation.FindAllAsync(
               GattDeviceService.GetDeviceSelectorFromUuid(MYO_IMU_UUID),
               new string[] { "System.Devices.ContainerId" });

            devicesCollection = await DeviceInformation.FindAllAsync(
                GattDeviceService.GetDeviceSelectorFromUuid(MYO_BATTERY_UUID),
                new string[] { "System.Devices.ContainerId" });

            devicesCollection = await DeviceInformation.FindAllAsync(
               GattDeviceService.GetDeviceSelectorFromUuid(MYO_CLASSIFIER_UUID),
               new string[] { "System.Devices.ContainerId" });

            if (devicesCollection.Count > 0)
            {

            }
        }

        /// <summary>
        /// Update Connect List
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="isConnect"></param>
        private void ConnectUpdate(MyoHw sender, bool isConnect)
        {
            if (isConnect)
            {
                connectedMyos.Add(sender);
            }
            else
            {
                connectedMyos.Remove(sender);
            }

        }


        /// <summary>
        /// Register to be notified when a connection is established to the Bluetooth device
        /// </summary>
        private void StartDeviceConnectionWatcher()
        {
            watcher = PnpObject.CreateWatcher(PnpObjectType.DeviceContainer,
                new string[] { "System.Devices.Connected" }, String.Empty);

            watcher.Updated += DeviceConnection_Updated;
            watcher.Start();
        }

        /// <summary>
        /// Invoked when a connection is established to the Bluetooth device
        /// </summary>
        /// <param name="sender">The watcher object that sent the notification</param>
        /// <param name="args">The updated device object properties</param>
        private async void DeviceConnection_Updated(PnpObjectWatcher sender, PnpObjectUpdate args)
        {
            var connectedProperty = args.Properties["System.Devices.Connected"];
            bool isConnected = false;

            List<MyoHw> myoSender = pairedMyos.Where(p => p.deviceContainerId == args.Id).ToList();

            if ((myoSender.Any()) && Boolean.TryParse(connectedProperty.ToString(), out isConnected) &&
                isConnected)
            {
                ConnectUpdate(myoSender[0], isConnected);
                // Notifying subscribers of connection state updates
                myoSender[0].OnConnectUpdate(isConnected);
            }
        }

    }
}
