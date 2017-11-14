using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.EF;

namespace Domain.Repositories
{
    public class ClientManager : IClientManager
    {
        public ApplicationContext Database { get; set; }
        public ClientManager(ApplicationContext app)
        {
            this.Database = app;
        }
        public void Create(ClientProfile item)
        {
            Database.ClientProfiles.Add(item);
        }

        public void Dispose()
        {
            Database.Dispose();
        }

        public IEnumerable<ClientProfile> GetAll()
        {
            return Database.ClientProfiles;
        }
    }
}
