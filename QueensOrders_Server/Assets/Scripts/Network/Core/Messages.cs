using System.IO;

public class MessageToSend
{
    public BinaryWriter w;
    public readonly MessageIdentifier id;

    public MessageToSend(MessageIdentifier idf, BinaryWriter on)
    {
        id = idf;
        w = on;
    }
}

public class MessageIdentifier
{
    public readonly byte id;
    public readonly int channel;

    public MessageIdentifier(byte _id, int _channel)
    {
        id = _id;
        channel = _channel;
    }

    public MessageToSend CreateMessage()
    {
        BinaryWriter bw = new BinaryWriter(new MemoryStream(128));
        bw.Write(id);
        return new MessageToSend(this, bw);
    }

    public MessageToSend CreateMessage(BinaryWriter on)
    {
        on.Write(id);
        return new MessageToSend(this, on);
    }
}