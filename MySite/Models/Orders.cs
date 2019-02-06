//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MySite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Orders
    {
        public int Id { get; set; }
        public int ID_Name { get; set; }
        public int ID_User { get; set; }
        
        public string About_Order { get; set; }
        public string Workman { get; set; }
        public Nullable<int> Salary_full { get; set; }
        public Nullable<int> Salary_workman { get; set; }
        public string Time { get; set; }

        [RegularExpression("^[а-яА-ЯёЁa-zA-Z0-9]+$", ErrorMessage = "*Статус может состоять только из букв и цифр")]
        public string Status { get; set; }
    }
}
