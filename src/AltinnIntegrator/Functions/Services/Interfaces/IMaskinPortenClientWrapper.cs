﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AltinnIntegrator.Services.Interface
{
    public interface IMaskinPortenClientWrapper
    {
        bool PostToken(FormUrlEncodedContent bearer, out string token); 
    }
}