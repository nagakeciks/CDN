namespace HafizDemoAPI
{
    public interface iChatClient
    {
        Task RefreshUserList();
        Task ReceiveMessage(string message);

        Task ReceiveMessageRoom(string From,string message);

        Task ReceiveMessageFromID(string From,string message);

        Task ReturnID(string ID);
    }
}
