using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;



namespace ZeroMeter
{
    public class USBWr
    {
        public static DateTime LastDataEventDate = DateTime.Now;


        public USBWr(int vid, int pid)
        {
            ////16700 0x413C
            ////16388 0x404

            //MyUsbFinder = new UsbDeviceFinder(vid, pid);
            //ErrorCode ec = ErrorCode.None;

            //try
            //{

            //    // Find and open the usb device.

            //    MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);
            //    // MyUsbDevice = UsbGlobals.OpenUsbDevice(MyUsbFinder);


            //    // If the device is open and ready
            //    if (MyUsbDevice == null) throw new Exception("Device Not Found.");

            //    // Select the configuration to use. This isn't needed with a winusb device. 
            //    // WinUsb sets the config to 1 when it's opened.
            //    //  ?????? MyUsbDevice.SetConfiguration(1);

            //    // Claim the first Interface. This isn't needed with a winusb device. 
            //    if (MyUsbDevice is LibUsbDevice) ((LibUsbDevice)MyUsbDevice).ClaimInterface(0);

            //    // open read endpoint 1.
            //    reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

            //    // open write endpoint 1.
            //    writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
            //    reader.DataReceived += (OnRxEndPointData);
            //    reader.DataReceivedEnabled = true;

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            //    if (MyUsbDevice != null) { MyUsbDevice.Close(); }

            //    MyUsbDevice = null;
            //}


        }
        ~USBWr()
        {
            //if (reader != null)
            //{
            //    reader.DataReceived -= (OnRxEndPointData);
            //    reader.DataReceivedEnabled = false;
            //}
            //if (MyUsbDevice != null) MyUsbDevice.Close();
            //MyUsbDevice = null;

        }
        //public UsbEndpointWriter GetUsbEndpointWriter()
        //{
        //    //return writer;
        //}
        //public UsbEndpointReader GetUsbEndpointReader()
        //{
        //    //return reader;
        //}
        //public static void Write(String strMess)
        //{
        //    //ErrorCode ec = ErrorCode.None;
        //    //int bytesWritten;
        //    //ec = writer.Write(Encoding.Default.GetBytes(strMess), 2000, out bytesWritten);
        //    ////if (ec != ErrorCode.None) throw new Exception(UsbGlobals.LastErrorString);
        //    //if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);

        //    ///*
        //    //            LastDataEventDate = DateTime.Now;
        //    //            while ((DateTime.Now - LastDataEventDate).TotalMilliseconds < 100)
        //    //            {
        //    //            }
        //    //*/
        //}

        //private static void OnRxEndPointData(object sender, EndpointDataEventArgs e)
        //{
        //    //LastDataEventDate = DateTime.Now;
        //    //// ?????? Program.LogString("Received: " + Encoding.Default.GetString(e.Buffer, 0, e.Count));
        //}

    }
}
