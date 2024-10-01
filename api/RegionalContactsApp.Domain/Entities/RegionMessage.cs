﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalContactsApp.Domain.Entities
{
    public class RegionMessage
    {
        public string Operation { get; set; }  // Tipo de operação: Create, Update ou Delete
        public Region Region { get; set; }   // Dados do contato
    }
}