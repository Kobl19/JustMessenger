using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите ваше имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введите вашу фамилию")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Введите електронный адрес")]
        [RegularExpression(@"(?i)\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b", ErrorMessage = "Email адрес указан не правильно")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Следует указать пароль от 5 до 20 символов")]
        [Compare("PasswordConfirm", ErrorMessage = "Пароли не совпадают")]
        public string Password { get; set; }

        public string PasswordConfirm { get; set; }
    }
}