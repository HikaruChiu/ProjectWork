﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PJW.Web
{
    public class MVCFile
    {
        public List<IFormFile> Files { get; private set; } = new List<IFormFile>();
    }
}
