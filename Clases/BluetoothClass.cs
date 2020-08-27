using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand;
using InTheHand.Net;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;

//using InTheHand.Devices.Enumeration;


namespace BTLibrary
{
    public class Enviar
    {

        private  BluetoothRadio myBLT ;
        private  BluetoothEndPoint localEndpoint;
        //client is used to manage connections
        private BluetoothClient localClient = new BluetoothClient();
        // Dispositivo remoto que nos solicita conexion.
        public BluetoothClient remoteDevice = new BluetoothClient();
        // component is used to manage device discovery
        private BluetoothComponent localComponent = new BluetoothComponent();
        // Listener para atender conexiones
        public BluetoothListener localListener;
        //Lista de dispositivos emparejados con este
        public BluetoothDeviceInfo[] paired ;
        // Nombre del dispositivo desde el que admitiremos la conexión
        public string remoteDeviceNameAdmited = "";

        private ZeroTrip.Utiles util = new ZeroTrip.Utiles();


        
        public Enviar()
        {

            myBLT = BluetoothRadio.PrimaryRadio;

            localEndpoint = new BluetoothEndPoint(myBLT.LocalAddress, BluetoothService.SerialPort, 6);

            localListener = new BluetoothListener(myBLT.LocalAddress, BluetoothService.SerialPort);

            localListener.Start();
            //localListener.BeginAcceptBluetoothClient(new AsyncCallback(AcceptConnection), localListener);

            paired = localClient.DiscoverDevices(255, false, true, false, false);
        }

        public void AcceptConnection(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                remoteDevice = ((BluetoothListener)result.AsyncState).EndAcceptBluetoothClient(result);
            }

            if (remoteDevice.RemoteMachineName != remoteDeviceNameAdmited)
            {
                util.AvisoConError("Hemos recibido un intento de conexión desde el dispositivo " + remoteDevice.RemoteMachineName +
                    " que no es el seleccionado " + remoteDeviceNameAdmited, "Intento de conexión Bluetooth");
                remoteDevice.Dispose();
            }
            else
            {

            }

            // Lo siguiente es para recibir datos desde el remoto

            //else
            //{ 
            //    Stream stream = remoteDevice.GetStream();

            //    while (remoteDevice.Connected)
            //    {
            //        try
            //        {
            //            byte[] byteReceived = new byte[1024];
            //            int read = stream.Read(byteReceived, 0, byteReceived.Length);
            //            if (read > 0)
            //            {
            //                Console.WriteLine("Messagem Recebida: " + Encoding.ASCII.GetString(byteReceived).Substring(0, read) + System.Environment.NewLine);
            //            }
            //            stream.Flush();
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine("Erro: " + e.ToString());
            //        }
            //    }
            //    stream.Close();
            //}
        }

        public BluetoothDeviceInfo[] listPaired()
        {
            //Devolvemos la lista de dispositivos emparejados.
           
            return paired;

            // check every discovered device if it is already paired 
            //foreach (BluetoothDeviceInfo device in paired)
            //{
            //    if (device.Authenticated)
            //    {
            //        if (device.DeviceName.Equals("MAMD"))
            //            // set pin of device to connect with
            //            // localClient.SetPin(DEVICE_PIN);
            //            // async connection method
            //            localClient.BeginConnect(device.DeviceAddress, BluetoothService.SerialPort, new AsyncCallback(Connect), device);
            //    }

            //}

        }
        // callback
        private void Connect(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                // client is connected now :)
                int a = 1;
            }
        }
        private void component_DiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
        {


        }

        public bool EnviarDatos(BluetoothClient device, string content)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException("content");
            }

            // for not block the UI it will run in a different threat
           // var task = Task.Run(() =>
            {
                //using (var bluetoothClient = new BluetoothClient())
                {
                    try
                    {

                        //var ep = new BluetoothEndPoint(device.DeviceInfo.DeviceAddress, _serviceClassId);
                        //var ep = new BluetoothEndPoint(myBLT.LocalAddress, BluetoothService.SerialPort);

                    
                        // get stream for send the data
                        // var bluetoothStream = bluetoothClient.GetStream();
                        var bluetoothStream = device.GetStream();

                        // if all is ok to send
                        //if (bluetoothClient.Connected && bluetoothStream != null)
                        if (device.Connected && bluetoothStream != null)
                        {
                            // write the data in the stream
                            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                           bluetoothStream.Write(buffer, 0, buffer.Length);
                            bluetoothStream.Flush();
                           // bluetoothStream.Close();
                            
                            return true;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        util.AvisoConEx(ex, "Puerto" + " no disponible o no existe", "Error en puerto");
                        return false;
                    }
                }
                //return false;
            }
           
        }


        public async Task<bool> Send(BluetoothClient device, string content)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException("content");
            }

            // for not block the UI it will run in a different threat
            var task = Task.Run(() =>
            {
                using (var bluetoothClient = new BluetoothClient())
                {
                    try
                    {
                        
                        //var ep = new BluetoothEndPoint(device.DeviceInfo.DeviceAddress, _serviceClassId);
                        var ep = new BluetoothEndPoint(myBLT.LocalAddress, BluetoothService.SerialPort);

                        // connecting
                        bluetoothClient.Connect(ep);

                        // get stream for send the data
                       // var bluetoothStream = bluetoothClient.GetStream();
                        var bluetoothStream = device.GetStream();

                        // if all is ok to send
                        //if (bluetoothClient.Connected && bluetoothStream != null)
                        if (device.Connected && bluetoothStream != null)
                            {
                            // write the data in the stream
                            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                            bluetoothStream.Write(buffer, 0, buffer.Length);
                            bluetoothStream.Flush();
                            bluetoothStream.Close();
                            return true;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        // the error will be ignored and the send data will report as not sent
                        // for understood the type of the error, handle the exception
                        util.AvisoConEx(ex, "Puerto" + " no disponible o no existe", "Error en puerto");
                    }
                }
                return false;
            });
            return await task;
        }
    


    }
}
