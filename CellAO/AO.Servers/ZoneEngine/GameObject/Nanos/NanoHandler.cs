
#region Usings...
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ComponentAce.Compression.Libs.zlib;
using MsgPack.Serialization;
#endregion

namespace ZoneEngine.GameObject.Nanos
{
    /// <summary>
    /// Item handler class
    /// </summary>
    public class NanoHandler
    {

        /// <summary>
        /// Cache of all item templates
        /// </summary>
        public static List<NanoFormula> NanoList = new List<NanoFormula>();

        /// <summary>
        /// Cache all item templates
        /// </summary>
        /// <returns>number of cached items</returns>
        public static int CacheAllNanos()
        {
            DateTime _now = DateTime.Now;
            NanoList = new List<NanoFormula>();
            Stream sf = new FileStream("nanos.dat", FileMode.Open);
            MemoryStream ms = new MemoryStream();

            ZOutputStream sm = new ZOutputStream(ms);
            CopyStream(sf, sm);

            ms.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[4];
            ms.Read(buffer, 0, 4);
            int packaged = BitConverter.ToInt32(buffer, 0);

            BinaryReader br = new BinaryReader(ms);
            var bf = MessagePackSerializer.Create<List<NanoFormula>>();

            while (true)
            {
                List<NanoFormula> templist = bf.Unpack(ms);
                NanoList.AddRange(templist);
                if (templist.Count != packaged)
                {
                    break;
                }
                Console.Write("Loaded {0} Nanos in {1}\r",
                              new object[] { NanoList.Count, new DateTime((DateTime.Now - _now).Ticks).ToString("mm:ss.ff") });
            }
            GC.Collect();
            return NanoList.Count;
        }

        /// <summary>
        /// Cache all item templates
        /// </summary>
        /// <returns>number of cached items</returns>
        public static int CacheAllNanos(string fname)
        {
            DateTime _now = DateTime.Now;
            NanoList = new List<NanoFormula>();
            Stream sf = new FileStream(fname, FileMode.Open);
            MemoryStream ms = new MemoryStream();

            ZOutputStream sm = new ZOutputStream(ms);
            CopyStream(sf, sm);

            ms.Seek(0, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(ms);
            var bf = MessagePackSerializer.Create<List<NanoFormula>>();


            byte[] buffer = new byte[4];
            ms.Read(buffer, 0, 4);
            int packaged = BitConverter.ToInt32(buffer, 0);

            while (true)
            {
                List<NanoFormula> templist = (List<NanoFormula>)bf.Unpack(ms);
                NanoList.AddRange(templist);
                if (templist.Count != packaged)
                {
                    break;
                }
                Console.Write("Loaded {0} nanos in {1}\r",
                              new object[] { NanoList.Count, new DateTime((DateTime.Now - _now).Ticks).ToString("mm:ss.ff") });
            }
            GC.Collect();
            return NanoList.Count;
        }


        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[2097152];
            int len;
            while ((len = input.Read(buffer, 0, 2097152)) > 0)
            {
                output.Write(buffer, 0, len);
                Console.Write("\rDeflating " + Convert.ToInt32(Math.Floor((double)input.Position / input.Length * 100.0)) +
                              "%");
            }
            output.Flush();
            Console.Write("\r                                             \r");
        }


        /// <summary>
        /// Returns a nano object
        /// </summary>
        /// <param name="id">ID of the nano</param>
        /// <returns>Nano</returns>
        public static NanoFormula GetNano(int id)
        {
            foreach (NanoFormula nanoFormula in NanoList)
            {
                if (nanoFormula.ID == id)
                    return nanoFormula;
            }
            return null;
        }
    }
}