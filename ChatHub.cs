using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using HafizDemoAPI.ModelCDN;


namespace HafizDemoAPI
{
    public sealed class ChatHub :Hub<iChatClient>
    {
        //public override async Task OnConnectedAsync()
        //{
        //    await Clients.All.ReceiveMessage("hello");
        //    //return base.OnConnectedAsync();
        //}
        public override async Task OnConnectedAsync()
        {
            var ConnID = Context.ConnectionId;
            //await Clients.Client(ConnID).ReceiveMessage($"Conn ID {ConnID}");

            await Clients.Client(ConnID).ReturnID(ConnID);
            
            // return base.OnConnectedAsync();
        }
        public  override Task OnDisconnectedAsync(Exception? exception)
        {
            var ConnID = Context.ConnectionId;
            CDNContext context = new();    
            var user = context.CDNUserConn.Where(d=> d.ConnectionId == ConnID).FirstOrDefault();
            if (user != null)
            {
                user.Connected = false;
                context.SaveChanges();
            }
           // Clients.All.RefreshUserList();
            return base.OnDisconnectedAsync(exception);
        }

        public async Task StatUpdate(string Group)
        {
            await Clients.All.NotifyStatUpdate(Group);
        }
        public async Task SendMessage(string Message)
        {
            await Clients.All.ReceiveMessage(Message);
        }

        public async Task SendMessageToRoom(Int32 UserID, string Message)
        {
            CDNContext cdnContext = new();
            var userMsg = cdnContext.Users.Where(user => user.UserId == UserID).FirstOrDefault();
            if (userMsg != null)
            {
                await Clients.All.ReceiveMessageRoom(userMsg.Username, Message);
            }

        }

        public async Task SendMessageToUser(Int32 FromID, Int32 ToID,string Message)
        {
            CDNContext cdnContext = new();
            var userCDN = cdnContext.CDNUserConn.Where(user => user.UserID == ToID).OrderByDescending(m => m.Id).FirstOrDefault();
            if (userCDN != null)
            {
                if (!userCDN.Connected)
                {
                    throw new Exception("User not online");
                }
                var userMsg = cdnContext.Users.Where(user => user.UserId == FromID).FirstOrDefault();
                if (userMsg != null)
                {
                    await Clients.Client(userCDN.ConnectionId).ReceiveMessageFromID(userMsg.Username, Message);
                }
                else
                {
                    throw new Exception("User not online");
                }
            }
            else
            {
                throw new Exception($"Invalid ID : {ToID.ToString()}");
            }
        }


        public async Task<string> MapCDNUserID(string ConnectionID, Int32 UserID)
        {
            try
            {
                CDNContext cdnContext = new ();
                var userCDN = cdnContext.CDNUserConn.Where(user => user.UserID == UserID).OrderByDescending(m => m.Id).FirstOrDefault();
                if (userCDN != null)
                {
                    //userTAeF.Connected = false;
                    userCDN.ConnectionId = ConnectionID;
                    userCDN.DateConnect = DateTime.Now;
                    userCDN.Connected = true;
                    await cdnContext.SaveChangesAsync();
                }
                else
                {
                    CDNUserConn user = new();
                    user.UserID = UserID;
                    user.ConnectionId = ConnectionID;
                    user.DateConnect = DateTime.Now;
                    user.Connected = true;
                    cdnContext.CDNUserConn.Add(user);
                    await cdnContext.SaveChangesAsync();
                }
                await Clients.All.RefreshUserList();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
