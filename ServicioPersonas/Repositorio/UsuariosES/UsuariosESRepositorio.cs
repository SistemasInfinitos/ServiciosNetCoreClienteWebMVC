using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServicioPersonas.Configuration;
using ServicioPersonas.ModelsAPI.Comun;
using ServicioPersonas.ModelsAPI.DataTable;
using ServicioPersonas.ModelsAPI.Persona;
using ServicioPersonas.ModelsDB;
using ServicioPersonas.ModelsDB.Contexts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioPersonas.Repositorio.UsuariosES
{
    public class UsuariosESRepositorio : IUsuariosESRepositorio
    {
        private readonly JwtConfiguracion _jwtConfig;
        private readonly Context _context;

        public UsuariosESRepositorio(IOptionsMonitor<JwtConfiguracion> optionsMonitor, Context context)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _context = context;
        }

        private readonly CultureInfo culture = new CultureInfo("is-IS");
        private readonly CultureInfo cultureFecha = new CultureInfo("en-US");

        public async Task<bool> ActualizarUsuario(UsuariosModel entidad)
        {
            bool ok = false;
            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    Usuario actualizarRegistro = _context.Usuarios.Where(x => x.id == entidad.id).FirstOrDefault();

                    if (actualizarRegistro != null)
                    {
                        actualizarRegistro.nombreUsuario = entidad.nombreUsuario;
                        actualizarRegistro.passwordHash = entidad.passwordHash; // requiere encriptacion -- no lo encripto porque es una prueba
                        actualizarRegistro.personaId = entidad.personaId;
                        actualizarRegistro.estado = true;
                        actualizarRegistro.fechaCreacion = DateTime.Now;
                        actualizarRegistro.fechaActualizacion = null;

                        actualizarRegistro.fechaActualizacion = DateTime.Now;

                        _context.Entry(actualizarRegistro).State = EntityState.Modified;
                        ok = await _context.SaveChangesAsync() > 0;
                    }

                    if (ok)
                    {
                        DbTran.Commit();
                    }
                }
                catch (Exception x)
                {
                    DbTran.Rollback();
                }
            }
            return await Task.Run(() => ok);
        }

        public async Task<bool> CrearUsuario(UsuariosModel entidad)
        {
            bool ok = false;

            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var verificarExiste = _context.Usuarios.Where(x => x.nombreUsuario == entidad.nombreUsuario).FirstOrDefault();
                    Usuario nuevoRegistro = new Usuario();
                    if (verificarExiste == null)
                    {
                        nuevoRegistro.nombreUsuario = entidad.nombreUsuario;
                        nuevoRegistro.passwordHash = entidad.passwordHash;// requiere encriptacion -- no lo encripto porque es una prueba
                        nuevoRegistro.personaId = entidad.personaId;
                        nuevoRegistro.estado = true;
                        nuevoRegistro.fechaCreacion = DateTime.Now;
                        nuevoRegistro.fechaActualizacion = null;

                        _context.Usuarios.Add(nuevoRegistro);
                        ok = await _context.SaveChangesAsync() > 0;
                    }

                    if (ok)
                    {
                        DbTran.Commit();
                    }
                }
                catch (Exception x)
                {
                    DbTran.Rollback();
                }
            }
            return await Task.Run(() => ok);
        }

        public async Task<UsuariosModel> GetUsuario(string buscar, int? Id)
        {
            UsuariosModel persona = new UsuariosModel();
            try
            {
                var predicado = PredicateBuilder.True<Usuario>();
                var predicado2 = PredicateBuilder.False<Usuario>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.nombreUsuario.Contains(buscar));
                    predicado = predicado.And(predicado2);
                }
                if (Id != null)
                {
                    predicado = predicado.And(x => x.id == Id);
                }
                var data = _context.Usuarios.Where(predicado).FirstOrDefault();
                if (data != null)
                {
                    persona.id = data.id;
                    persona.nombreUsuario = data.nombreUsuario;
                    persona.personaId = data.personaId;
                    persona.estado = true;
                    persona.fechaCreacion = data.fechaCreacion.ToString("yyyy/MM/dd", cultureFecha);
                    persona.fechaActualizacion = data.fechaActualizacion != null ? data.fechaActualizacion.Value.ToString("yyyy/MM/dd", cultureFecha) : "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return await Task.Run(() => persona);
        }

        public async Task<DataTableResponseUsuario> GetUsuariosDataTable(DataTableParameter dtParameters)
        {
            try
            {
                DataTableResponseUsuario datos = new DataTableResponseUsuario();
                string search = dtParameters.search?.value;
                search = search.Replace(" ", "");
                List<string> sortcolumn2 = new List<string>();
                string sortcolumn3 = "";

                if (dtParameters != null && dtParameters.order != null && dtParameters.order.Count() > 0)
                {
                    foreach (var id in dtParameters.order)
                    {
                        sortcolumn2.Add(dtParameters.columns[id.column].name);
                        sortcolumn3 += (dtParameters.columns[id.column].name) + ",";
                    }
                }
                string sortcolumn = dtParameters.columns[dtParameters.order[0].column].name;

                var predicado = PredicateBuilder.True<Usuario>();
                var predicado2 = PredicateBuilder.False<Usuario>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(dtParameters.search.value))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.nombreUsuario.Contains(dtParameters.search.value));
                    predicado = predicado.And(predicado2);
                }

                datos.recordsFiltered = _context.Usuarios.Where(predicado).ToList().Count();
                datos.recordsTotal = datos.recordsFiltered;
                datos.draw = dtParameters.draw;

                if (dtParameters.length == -1)
                {
                    dtParameters.length = datos.recordsFiltered;
                }
                string order = "asc";
                if (dtParameters.order.Count > 0)
                {
                    order = dtParameters.order?[0].dir;
                }
                if (string.IsNullOrWhiteSpace(sortcolumn))
                {
                    sortcolumn = "nombreUsuario";
                }
                List<Usuario> datos2 = new List<Usuario>();
                if (datos.recordsFiltered > 0)
                {
                    datos2 = _context.Usuarios.Where(predicado).OrderBy2(sortcolumn, order).Skip(dtParameters.start).Take(dtParameters.length).ToList();
                    datos.data = datos2.Select(x => new UsuariosModel
                    {
                        id = x.id,
                        nombreUsuario = x.nombreUsuario,
                        fechaActualizacion = x.fechaActualizacion != null ? x.fechaActualizacion.Value.ToString("yyyy/MM/dd", culture) : "",
                        fechaCreacion = x.fechaCreacion.ToString("yyyy/MM/dd", culture),
                        personaId=x.personaId,
                        //persona= x.persona!=null?x.persona.nombres +" "+ x.persona.apellidos:"" esto deberia tener una vista para traer todo los datos y por rendimiento
                    }).ToList();
                }

                return await Task.Run(() => datos);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteUsuario(int id)
        {
            bool ok = false;
            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var delete = _context.Usuarios.Where(x => x.id == id).FirstOrDefault();

                    if (delete != null)
                    {
                        _context.Entry(delete).State = EntityState.Deleted;
                        ok = await _context.SaveChangesAsync() > 0;
                    }
                    if (ok)
                    {
                        DbTran.Commit();
                    }
                }
                catch (Exception x)
                {
                    DbTran.Rollback();
                }
            }
            return await Task.Run(() => ok);
        }

        public async Task<List<DropListModel>> GetUsuariosDropList(string buscar, int? id)
        {
            List<DropListModel> datos = new List<DropListModel>();
            try
            {
                var predicado = PredicateBuilder.True<Usuario>();
                var predicado2 = PredicateBuilder.False<Usuario>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.nombreUsuario.Contains(buscar));
                    predicado = predicado.And(predicado2);
                }
                if (id != null)
                {
                    predicado = predicado.And(d => d.id == id);
                }

                var data = _context.Usuarios.Where(predicado).Take(10).ToList();

                datos = data.Select(x => new DropListModel
                {
                    id = x.id,
                    text = x.nombreUsuario
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return await Task.Run(() => datos);
        }
    }
}