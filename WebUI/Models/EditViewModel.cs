using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Models
{
    public class EditViewModel
    {
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "ID")]
        public string Id { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name ="Адрес почты")]
        [RegularExpression(@"(?i)\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b", ErrorMessage = "Email адрес указан не правильно")]
        public string Email { get; set; }

        [Display(Name = "Старый пароль")]
        [StringLength(20, MinimumLength =5, ErrorMessage = "Следует указать пароль от 5 до 20 символов")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Новый пароль")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Следует указать пароль от 5 до 20 символов")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Статус")]
        public string Status { get; set; }

        [Display(Name = "Пол")]
        public string Sex { get; set; }
        public string ExternalUrl { get; set; }
        public string InternalUrl { get; set; }
    }
}