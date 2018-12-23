﻿using MyRestaurantApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurantApp.ViewModel
{
    public class IndexViewModel
    {
        public IEnumerable<MenuItem> MenuItems { get; set; }
        public IEnumerable<CategoryType> CategoryTypes { get; set; }
    }
}
