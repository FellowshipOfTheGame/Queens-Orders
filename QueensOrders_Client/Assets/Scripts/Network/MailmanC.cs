using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BitMap8 : List<byte> { };

public class MailmanC
{
    private static MailmanC mailman = null;
    private List<SyncableObject> objects; ///< All syncable objects in game

    private MailmanC()
    {
        objects = new List<SyncableObject>();
    }

    public void Fetch(MemoryStream stream)
    {
        //
        BinaryReader reader = new BinaryReader(stream);

        while (reader.PeekChar() >= 0)
        {
            ushort index = reader.ReadUInt16();
            byte mask = reader.ReadByte();

            if (index < objects.Count){
                objects[index].ReadFromBuffer(reader, mask);
            }
        }


    }

    public static MailmanC Instance()
    {
        if (mailman == null) {
            mailman = new MailmanC();
        }

        return mailman;
    }

    public int UnitCreated(UnitSyncC s)
    {
        objects.Add(s);
        return objects.Count;
    }

}
