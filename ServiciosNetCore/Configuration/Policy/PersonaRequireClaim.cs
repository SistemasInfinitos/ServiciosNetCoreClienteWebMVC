﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiciosNetCore.Configuration.Policy
{
    public class PersonaRequireClaim : IAuthorizationRequirement
    {
    }
}
