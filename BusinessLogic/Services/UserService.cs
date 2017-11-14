using BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DTO;
using BusinessLogic.Infastracture;
using System.Security.Claims;
using Domain.Interfaces;
using Domain.Entities;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using AutoMapper;
using System.Web;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        IUnitOfWork Database { get; set; }
        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }
      

        public async Task<OperationDetails> Create(UserDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user==null)
            {
                user = new ApplicationUser { Email = userDto.Email, UserName = userDto.Email };
                var result = await Database.UserManager.CreateAsync(user, userDto.Password);
                if (result.Errors.Count() > 0)
                    return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
                
                // добавляем роль
                await Database.UserManager.AddToRoleAsync(user.Id, userDto.Role);
                
                // создаем профиль клиента
                ClientProfile clientProfile = new ClientProfile { Id=user.Id,FirstName=userDto.FirstName, LastName=userDto.LastName, Email=userDto.Email, Date=DateTime.Now };
                Database.ClientManager.Create(clientProfile);
                await Database.SaveAsync();
                
                return new OperationDetails(true, "Регистрация успешно пройдена", "");
            }
            else
            {
                return new OperationDetails(false, "Пользователь с таким логином уже существует", "Email");
            }
        }
        public async Task<OperationDetails> Edit(UserDTO userDto, string Id)
        {
            IEnumerable<ClientProfile> clientProfile = Database.ClientManager.GetAll();
            ClientProfile client=clientProfile.Where(x => x.Id == Id).FirstOrDefault();
            ApplicationUser user = await Database.UserManager.FindByIdAsync(Id);
            if (client != null)
            {
                if (userDto.FirstName != null)
                    client.FirstName = userDto.FirstName;
                if (userDto.LastName != null)
                    client.LastName = userDto.LastName;
                if (userDto.Email != null)
                {
                    client.Email = userDto.Email;
                    user.Email = userDto.Email;
                    user.UserName = userDto.Email;
                }
                if (userDto.Status != null)
                    client.Status = userDto.Status;
                if (userDto.Sex != null)
                    client.Sex = userDto.Sex;
                if (userDto.InternalUrl != null)
                    client.InternalUrl = userDto.InternalUrl;


                await Database.SaveAsync();
                await Database.UserManager.UpdateAsync(user);

                return new OperationDetails(true, "Изменения успешно сохранены", "");
            }
            return new OperationDetails(false, "Пользователь не существует", "Email");
        }
        public async Task<OperationDetails> ChangePassword(UserDTO userDto, string Id)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(Id);
            ApplicationUser usersPassword = await Database.UserManager.FindAsync(user.UserName, userDto.OldPassword);

            if (user!=null)
            {
                if (usersPassword==null)
                {
                    return new OperationDetails(false, "Сменить пароль не удалось. Попробуйте еще раз", " ");
                }
                if (userDto.NewPassword != null)
                {
                    var Details= await Database.UserManager.ChangePasswordAsync(Id, userDto.OldPassword, (string)userDto.NewPassword);
                    
                        return new OperationDetails(true, "Пароль был изменен успешно", "");
                    
                }
            }
            return new OperationDetails(false, "Пользователя не найдено", "Email");
        }
        public async Task<ClaimsIdentity> Authenticate(UserDTO userDto)
        {
            ClaimsIdentity claim = null;
            // находим пользователя
            ApplicationUser user = await Database.UserManager.FindAsync(userDto.Email, userDto.Password);
            // авторизуем его и возвращаем объект ClaimsIdentity
            if (user != null)
                claim = await Database.UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
            return claim;
        }

        // начальная инициализация бд
        public async Task SetInitialData(UserDTO adminDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                var role = await Database.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole { Name = roleName };
                    await Database.RoleManager.CreateAsync(role);
                }
            }
            await Create(adminDto);
        }


        public void Dispose()
        {
            Database.Dispose();
        }

        public IEnumerable<UserDTO> usersDTO()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<ClientProfile, UserDTO>());
            return Mapper.Map < IEnumerable<ClientProfile>, List<UserDTO>>(Database.ClientManager.GetAll());
        }
    }
}
