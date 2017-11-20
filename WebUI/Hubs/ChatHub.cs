using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using WebUI.Models;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.Globalization;
using BusinessLogic.Interfaces;

namespace WebUI.Hubs
{
   
    public class ChatHub : Hub
    {
        //static List<User> Users = new List<User>();
        
        // Отправка сообщений
        public void Send(string name, string message)
        {
           
            
            Clients.All.SendMessage(name, message);
        }

        
    
    }
}