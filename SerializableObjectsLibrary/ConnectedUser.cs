using System;

namespace SerializableObjectsLibrary
{

    [Serializable]
    public class ConnectedUser
    {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public ConnectedUser(string name, string connectionId)
        {
            Name = name;
            ConnectionId = connectionId;
        }
    }
}
