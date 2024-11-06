public class Config
{
    public Inbound[] Inbounds { get; set; }
    public Outbound[] Outbounds { get; set; }
    public Routing Routing { get; set; }
}

public class Inbound
{
    public int Port { get; set; }
    public string Listen { get; set; }
    public string Protocol { get; set; }
    public string Encryption { get; set; }
    public InboundSettings Settings { get; set; }
}

public class InboundSettings
{
    public string Id { get; set; } // UUID
    public string ServerAddress { get; set; } // ServerAddress
    public string PublicKey { get; set; } // PublicKey
    public string Label { get; set; } // Label
}

public class Outbound
{
    public string Protocol { get; set; }
    public string Encryption { get; set; }
    public OutboundSettings Settings { get; set; }
    public StreamSettings StreamSettings { get; set; }
}

public class OutboundSettings
{
    public Vnext[] Vnext { get; set; }
}

public class Vnext
{
    public string Address { get; set; }
    public int Port { get; set; }
    public string Encryption { get; set; }
    public User[] Users { get; set; }
}

public class User
{
    public string Id { get; set; }
    public int AlterId { get; set; }
    public string Security { get; set; }
    public string Fingerprint { get; set; }
    public string Encryption { get; set; }
}

public class StreamSettings
{
    public string Network { get; set; }
    public string Encryption { get; set; }
    public string Security { get; set; }
    public RealitySettings RealitySettings { get; set; }
}

public class RealitySettings
{
    public string PublicKey { get; set; }
    public string ShortId { get; set; }
    public string SpiderX { get; set; }
    public string Fingerprint { get; set; }
    public string Encryption { get; set; }
}

public class Routing
{
    public RoutingRule[] Rules { get; set; }
}

public class RoutingRule
{
    public string Type { get; set; }
    public string OutboundTag { get; set; }
    public string Encryption { get; set; }
    public string[] Ip { get; set; }
}
