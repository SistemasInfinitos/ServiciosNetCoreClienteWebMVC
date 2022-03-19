using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiciosNetCore.Configuration;
using ServiciosNetCore.ModelsAPI.Comun;
using ServiciosNetCore.ModelsAPI.DataTable;
using ServiciosNetCore.ModelsAPI.Pedidos;
using ServiciosNetCore.ModelsAPI.Productos;
using ServiciosNetCore.ModelsDB;
using ServiciosNetCore.ModelsDB.Contexts;
using ServiciosNetCore.Repositorio.PedidosES;
using ServiciosNetCore.Repositorio.ProductosES;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ServiciosNetCore.Repositorio.ProcuctosES
{
    public class PedidosESRepositorio : IPedidosESRepositorio
    {
        private readonly JwtConfiguracion _jwtConfig;
        private readonly Context _context;

        public PedidosESRepositorio(IOptionsMonitor<JwtConfiguracion> optionsMonitor, Context context)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _context = context;
        }

        private readonly CultureInfo culture = new CultureInfo("is-IS");
        private readonly CultureInfo cultureFecha = new CultureInfo("en-US");

        public async Task<bool> ActualizarPedidoEncabezado(EncabezadoPedidosModel entidad)
        {
            bool ok = false;
            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    EncabezadoPedido actualizarRegistro = _context.EncabezadoPedidos.Where(x => x.id == entidad.id).FirstOrDefault();

                    if (actualizarRegistro != null)
                    {
                        actualizarRegistro.usuarioId = entidad.usuarioId;
                        actualizarRegistro.clientePersonaId = entidad.clientePersonaId;
                        actualizarRegistro.valorNeto = entidad.valorNeto;
                        actualizarRegistro.valorIva = entidad.valorIva;
                        actualizarRegistro.valorTotal = entidad.valorTotal;
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

        public async Task<bool> CrearPedido(EncabezadoPedidosModel entidad)
        {
            bool ok = false;

            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var verificarExiste = _context.EncabezadoPedidos.Where(x => x.id == entidad.id).FirstOrDefault();
                    EncabezadoPedido nuevoRegistro = new EncabezadoPedido();
                    if (verificarExiste == null)
                    {
                        nuevoRegistro.usuarioId = entidad.usuarioId;
                        nuevoRegistro.clientePersonaId = entidad.clientePersonaId;
                        nuevoRegistro.valorNeto = entidad.valorNeto;
                        nuevoRegistro.valorIva = entidad.valorIva;
                        nuevoRegistro.valorTotal = entidad.valorTotal;
                        nuevoRegistro.estado = true;
                        nuevoRegistro.fechaCreacion = DateTime.Now;
                        nuevoRegistro.fechaActualizacion = null;

                        _context.EncabezadoPedidos.Add(nuevoRegistro);
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

        public async Task<EncabezadoPedidosModel> GetPedido(string buscar, int? Id)
        {
            EncabezadoPedidosModel entidad = new EncabezadoPedidosModel();
            try
            {
                var predicado = PredicateBuilder.True<EncabezadoPedido>();
                var predicado2 = PredicateBuilder.False<EncabezadoPedido>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.clientePersonaId.ToString().Contains(buscar));
                    predicado = predicado.And(predicado2);
                }
                if (Id != null)
                {
                    predicado = predicado.And(x => x.id == Id);
                }
                var data = _context.EncabezadoPedidos.Where(predicado).FirstOrDefault();
                if (data != null)
                {
                    entidad.id = data.id;
                    entidad.estado = true;
                    entidad.fechaCreacion = data.fechaCreacion.ToString("yyyy/MM/dd", cultureFecha);
                    entidad.fechaActualizacion = data.fechaActualizacion != null ? data.fechaActualizacion.Value.ToString("yyyy/MM/dd", cultureFecha) : "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return await Task.Run(() => entidad);
        }

        public async Task<DataTableResponsePedido> GetPedidosDataTable(DataTableParameter dtParameters)
        {
            try
            {
                DataTableResponsePedido datos = new DataTableResponsePedido();
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

                var predicado = PredicateBuilder.True<EncabezadoPedido>();
                var predicado2 = PredicateBuilder.False<EncabezadoPedido>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(dtParameters.search.value))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.clientePersonaId.ToString().Contains(dtParameters.search.value));
                    predicado = predicado.And(predicado2);
                }

                datos.recordsFiltered = _context.EncabezadoPedidos.Where(predicado).ToList().Count();
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
                List<EncabezadoPedido> datos2 = new List<EncabezadoPedido>();
                if (datos.recordsFiltered > 0)
                {
                    datos2 = _context.EncabezadoPedidos.Where(predicado).OrderBy2(sortcolumn, order).Skip(dtParameters.start).Take(dtParameters.length).ToList();
                    datos.data = datos2.Select(x => new EncabezadoPedidosModel
                    {
                        id = x.id,
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

        public async Task<bool> DeletePedido(int id)
        {
            bool ok = false;
            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var delete = _context.EncabezadoPedidos.Where(x => x.id == id).FirstOrDefault();

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

        public async Task<List<DropListModel>> GetPedidosDropList(string buscar, int? id)
        {
            List<DropListModel> datos = new List<DropListModel>();
            try
            {
                var predicado = PredicateBuilder.True<EncabezadoPedido>();
                var predicado2 = PredicateBuilder.False<EncabezadoPedido>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.clientePersonaId.ToString().Contains(buscar));
                    predicado = predicado.And(predicado2);
                }
                if (id != null)
                {
                    predicado = predicado.And(d => d.id == id);
                }

                var data = _context.EncabezadoPedidos.Where(predicado).Take(10).ToList();

                datos = data.Select(x => new DropListModel
                {
                    id = x.id,
                    text = x.fechaCreacion.ToString("yyyy/MM/dd", culture)
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