﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MyCosts.Models
{
    public class Cost
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        [Display(Name = "Продукт")]
        public int? ProductId { get; set; }

        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Цена")]
        public decimal Sum { get; set; }

        [Display(Name = "Магазин")]
        public string Store { get; set; }

        [Display(Name = "Количество")]
        public int? Count { get; set; }

        [Display(Name = "Вес в килограммах")]
        public double? WeightInKg { get; set; }

        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
