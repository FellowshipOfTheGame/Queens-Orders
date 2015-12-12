using System.IO;

// The "final" send message
// This may be an inner message from a bigger message
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

// Define a "kind" of message with one channel and one id
// This is used to register on the NetworkServer a function to handle this kind of message.
public class MessageIdentifier
{
    public readonly byte id;
    public readonly int channel;

    public MessageIdentifier(int _channel, byte _id)
    {
        channel = _channel;
        id = _id;
    }

	// Creates a base message for this MessageIdentifier
    public MessageToSend CreateMessage()
    {
        BinaryWriter bw = new BinaryWriter(new MemoryStream(128));
        bw.Write(id);
        return new MessageToSend(this, bw);
    }

	// Create a base message inside the given BinaryWriter
	// Used to concatenate multiple levels of messages
	// message [ inner message with local protocol [ with an inner message that is handled by another system ] ]
	// Only the "first" message level is handled by the NetworkServer, because the network server only knows
	// the outer message protocol.
    public MessageToSend CreateMessage(BinaryWriter on)
    {
        on.Write(id);
        return new MessageToSend(this, on);
    }
}