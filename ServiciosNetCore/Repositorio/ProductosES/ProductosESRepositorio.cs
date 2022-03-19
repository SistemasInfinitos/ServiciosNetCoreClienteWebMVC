using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiciosNetCore.Configuration;
using ServiciosNetCore.ModelsAPI.Comun;
using ServiciosNetCore.ModelsAPI.DataTable;
using ServiciosNetCore.ModelsDB.Contexts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ServiciosNetCore.Repositorio.PersonasES
{
    public class ProductosESRepositorio : IProductosESRepositorio
    {
        private readonly JwtConfiguracion _jwtConfig;
        private readonly Context _context;

        public ProductosESRepositorio(IOptionsMonitor<JwtConfiguracion> optionsMonitor, Context context)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _context = context;
        }

        private readonly CultureInfo culture = new CultureInfo("is-IS");
        private readonly CultureInfo cultureFecha = new CultureInfo("en-US");

        public async Task<bool> ActualizarPersona(PersonasModel entidad)
        {
            bool ok = false;
            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    Persona actualizarRegistro = _context.Personas.Where(x => x.id == entidad.id).FirstOrDefault();

                    if (actualizarRegistro != null)
                    {
                        actualizarRegistro.nombres = entidad.nombres;
                        actualizarRegistro.apellidos = entidad.apellidos;
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

        public async Task<bool> CrearPersona(PersonasModel entidad)
        {
            bool ok = false;

            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var verificarExiste = _context.Personas.Where(x => x.nombres == entidad.nombres).FirstOrDefault();
                    Persona nuevoRegistro = new Persona();
                    if (verificarExiste == null)
                    {
                        nuevoRegistro.nombres = entidad.nombres;
                        nuevoRegistro.apellidos = entidad.apellidos;
                        nuevoRegistro.estado = true;
                        nuevoRegistro.fechaCreacion = DateTime.Now;
                        nuevoRegistro.fechaActualizacion = null;

                        _context.Personas.Add(nuevoRegistro);
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

        public async Task<PersonasModel> GetPersona(string buscar, int? Id)
        {
            PersonasModel persona = new PersonasModel();
            try
            {
                var predicado = PredicateBuilder.True<Persona>();
                var predicado2 = PredicateBuilder.False<Persona>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.nombres.Contains(buscar));
                    predicado2 = predicado2.Or(d => 1 == 1 && d.apellidos.Contains(buscar));
                    predicado = predicado.And(predicado2);
                }
                if (Id != null)
                {
                    predicado = predicado.And(x => x.id == Id);
                }
                var data = _context.Personas.Where(predicado).FirstOrDefault();
                if (data != null)
                {
                    persona.id = data.id;
                    persona.nombres = data.nombres;
                    persona.apellidos = data.apellidos;
                    persona.estado = true;
                    persona.fechaCreacion = data.fechaCreacion.ToString("yyyy/MM/dd", cultureFecha);
                    persona.fechaActualizacion = data.fechaActualizacion != null ? data.fechaActualizacion.Value.ToString("yyyy/MM/dd", cultureFecha) : "";
                    persona.cliente = data.nombres +  " " + data.apellidos;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return await Task.Run(() => persona);
        }

        public async Task<DataTableResponsePersona> GetPersonasDataTable(DataTableParameter dtParameters)
        {
            try
            {
                DataTableResponsePersona datos = new DataTableResponsePersona();
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

                var predicado = PredicateBuilder.True<Persona>();
                var predicado2 = PredicateBuilder.False<Persona>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(dtParameters.search.value))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.nombres.Contains(dtParameters.search.value));
                    predicado2 = predicado2.Or(d => 1 == 1 && d.apellidos.Contains(dtParameters.search.value));
                    predicado = predicado.And(predicado2);
                }

                datos.recordsFiltered = _context.Personas.Where(predicado).ToList().Count();
                datos.recordsTotal = datos.recordsFiltered;
                datos.draw = dtParameters.draw;

                if (dtParameters.length == -1)
                {
                    dtParameters.length = datos.recordsFiltered;
                }
                string order = "asc";
                if (dtParameters.order.Count >0)
                {
                    order = dtParameters.order?[0].dir;
                }
                if (string.IsNullOrWhiteSpace(sortcolumn))
                {
                    sortcolumn = "PrimerNombre";
                }
                List<Persona> datos2 = new List<Persona>();
                if (datos.recordsFiltered > 0)
                {
                    datos2 = _context.Personas.Where(predicado).OrderBy2(sortcolumn, order).Skip(dtParameters.start).Take(dtParameters.length).ToList();
                    datos.data = datos2.Select(x => new PersonasModel
                    {
                        id = x.id,
                        nombres = x.nombres,
                        apellidos = x.apellidos,
                        fechaActualizacion = x.fechaActualizacion != null ? x.fechaActualizacion.Value.ToString("yyyy/MM/dd", culture) : "",
                        fechaCreacion = x.fechaCreacion.ToString("yyyy/MM/dd", culture),
                    }).ToList();
                }


                return await Task.Run(() => datos);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeletePersona(int id)
        {
            bool ok = false;
            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var delete = _context.Personas.Where(x => x.id == id).FirstOrDefault();

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

        public async Task<List<DropListModel>> GetPersonasDropList(string buscar, int? id)
        {
            List<DropListModel> datos = new List<DropListModel>();
            try
            {
                var predicado = PredicateBuilder.True<Persona>();
                var predicado2 = PredicateBuilder.False<Persona>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.nombres.Contains(buscar));
                    predicado2 = predicado2.Or(d => 1 == 1 && d.apellidos.Contains(buscar));
                    predicado = predicado.And(predicado2);
                }
                if (id != null)
                {
                    predicado = predicado.And(d => d.id == id);
                }

                var data = _context.Personas.Where(predicado).Take(10).ToList();

                datos = data.Select(x => new DropListModel
                {
                    id = x.id,
                    text = x.nombres + " " + x.apellidos 
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