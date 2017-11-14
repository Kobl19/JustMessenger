using BusinessLogic.DTO;
using BusinessLogic.Infastracture;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IUserService:IDisposable
    {
        Task<OperationDetails> Create(UserDTO userDto);
        Task<OperationDetails> Edit(UserDTO userDto, string Id);
        Task<OperationDetails> ChangePassword(UserDTO userDto, string Id);
        Task<ClaimsIdentity> Authenticate(UserDTO userDto);
        Task SetInitialData(UserDTO adminDto, List<string> roles);
        IEnumerable<UserDTO> usersDTO();
    }
}
