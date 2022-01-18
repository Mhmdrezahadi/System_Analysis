using Socket;

namespace System_Analysis.Socket
{
    public class Chat
    {
        private readonly AppHub _appHub;
        public Chat(AppHub appHub)
        {
            _appHub = appHub;
        }

        public void SendMessage(Message message)
        {
            object[] obj = new object[1];
            _appHub.Clients.Caller.SendCoreAsync("OnRecieveMessage", obj);
           
        }
    }
    public class Message
    {
        public string TextMessage { get; set; }
        public FormFile File { get; set; }
    }
}
