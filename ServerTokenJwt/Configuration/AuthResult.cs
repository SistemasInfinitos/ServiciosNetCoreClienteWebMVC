using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerTokenJwt.Configuration
{
    public class AuthResult
    {
        public string token { get; set; }
        public bool success { get; set; }
        public List<string> errors { get; set; }
    }
}
