using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiceHexOutput
{

    public static class NiceHexOutput
    {
        public static string Output(byte[] packet)
        {
            if (packet == null) return string.Empty;
            string outp = "";
            int counter = 0;

            outp = "Packet length: " + packet.Length.ToString()+"\r\n";

            while (counter < packet.Length)
            {
                outp = outp + " ";
                if (packet.Length - counter > 16)
                {
                    byte[] temp = new byte[16];
                    Array.Copy(packet, counter, temp, 0, 16);
                    outp = outp + BitConverter.ToString(temp).Replace("-", " ").PadRight(52);
                    foreach (byte b in temp)
                    {
                        outp = outp + ToSafeAscii(b);
                    }
                    outp = outp + "\r\n";
                }
                else
                {
                    byte[] temp = new byte[packet.Length-counter];
                    Array.Copy(packet, counter, temp, 0, packet.Length - counter);
                    outp = outp + BitConverter.ToString(temp).Replace("-", " ").PadRight(52);
                    foreach (byte b in temp)
                    {
                        outp = outp + ToSafeAscii(b);
                    }
                    outp = outp + "\r\n";
                }
                counter += 16;
            }
            return outp;
        }


        public static char ToSafeAscii(int b)
        {
            if (b >= 32 && b <= 126)
            {
                return (char)b;
            }
            return '.';
        }
    }

}
