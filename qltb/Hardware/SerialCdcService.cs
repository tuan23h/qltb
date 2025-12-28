using System;
using System.IO.Ports;

namespace qltb.Hardware
{
    public class SerialCdcService
    {
        private SerialPort _port;

        public event EventHandler<QrScanEventArgs> QrScanned;

        public void Start(string com)
        {
            _port = new SerialPort(com, 115200);
            _port.DataReceived += (s, e) =>
            {
                string qr = _port.ReadLine().Trim();
                QrScanned?.Invoke(this, new QrScanEventArgs(qr));
            };
            _port.Open();
        }
    }
}
