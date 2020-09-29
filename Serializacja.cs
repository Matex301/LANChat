using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Serializacja
{
    [Serializable]
    public class Packet
    {
        public bool isOk;
        public Packettype PacketType;
        public List<string> Data;

        public Packet(Packettype type)
        {
            this.PacketType = type;
            Data = new List<string>();
        }

        public Packet(byte[] packetbytes)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(packetbytes);
                Packet p = (Packet)bf.Deserialize(ms);
                ms.Close();

                this.PacketType = p.PacketType;
                this.Data = p.Data;

                isOk = true;
            }
            catch (System.IO.IOException)
            {
                isOk = false;
            }
        }

        /*public bool TryPacket(byte[] packetbytes)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(packetbytes);
                Packet p = (Packet)bf.Deserialize(ms);
                ms.Close();

                this.PacketType = p.PacketType;
                this.Data = p.Data;

                return true;
            }
            catch(System.IO.IOException)
            {
                return false;
            }
        }*/

        public byte[] ToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            byte[] bytes = ms.ToArray();
            ms.Close();

            return bytes;
        }

        public enum Packettype
        {
            Registration,
            Message
        }
    }
}
