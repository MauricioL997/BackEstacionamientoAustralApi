﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Tarifa
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public decimal Valor { get; set; }
    }
}
