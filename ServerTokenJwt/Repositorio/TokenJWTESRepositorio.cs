
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServerTokenJwt.Configuration;
using ServerTokenJwt.ModelsDB;
using ServerTokenJwt.ModelsDB.Contexts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using ServerTokenJwt.ModelsAPI;
using System.Text;

namespace ServerTokenJwt.Repositorio
{
    public class TokenJWTESRepositorio : ITokenJWTESRepositorio
    {
        private readonly JwtConfiguracion _jwtConfig;
        private readonly Context _context;

        public TokenJWTESRepositorio(IOptionsMonitor<JwtConfiguracion> optionsMonitor, Context context)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _context = context;
        }

        private readonly CultureInfo culture = new CultureInfo("is-IS");
        private readonly CultureInfo cultureFecha = new CultureInfo("en-US");
        //private readonly static string key = "MBCSIstemasInfinitos4ñ1r550pñ1r9";//32
        //private readonly static string iv = "BElLosñljXZ9ñ5q*";
        public async Task<string> EncriptarContrasena(string contrasena)
        {
            try
            {
                var key = _jwtConfig.key;
                string iv = _jwtConfig.iv;
                byte[] plainttext = ASCIIEncoding.ASCII.GetBytes(contrasena);
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Key = ASCIIEncoding.ASCII.GetBytes(key);
                aes.IV = ASCIIEncoding.ASCII.GetBytes(iv);
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                ICryptoTransform crypto = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] encryptedtext = crypto.TransformFinalBlock(plainttext, 0, plainttext.Length);
                return await Task.Run(() => Convert.ToBase64String(encryptedtext));
            }
            catch (Exception ex)
            {
                var debug = ex.Message;
                return null;
            }
        }

        public async Task<UsuarioModel> Login(UsuarioModel User)
        {
            try
            {
                 string password = await EncriptarContrasena(User.pasword);
                 var Usuario = await Task.Run(() => _context.Usuarios.Include(i=>i.persona).Where(b => b.nombreUsuario == User.usuario && b.passwordHash == password).Select(x =>
                 new UsuarioModel
                 {
                     id = x.id,
                     roles = "Admin,Asesor",// los quemo porque es una prueba 
                     usuario= x.nombreUsuario,
                     fullName=(x.persona.nombres+""+ x.persona.apellidos)
                 }).FirstOrDefault());

                return await Task.Run(() => Usuario);
            }
            catch (EvaluateException ex)
            {
                var debug = ex.Message;
                return null;
            }            
        }
    }
}
