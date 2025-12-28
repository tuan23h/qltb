using System;

namespace qltb.Hardware
{
    public class QrScanEventArgs : EventArgs
    {
        public string QrCode { get; }

        public QrScanEventArgs(string qr)
        {
            QrCode = qr;
        }
    }
}
