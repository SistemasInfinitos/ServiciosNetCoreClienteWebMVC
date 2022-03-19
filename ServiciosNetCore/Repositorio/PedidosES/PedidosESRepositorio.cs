﻿using Microsoft.Data.SqlClient;
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
using System.Data;
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
                        actualizarRegistro.usuarioId = entidad.usuarioId.Value;
                        actualizarRegistro.clientePersonaId = entidad.clientePersonaId;
                        actualizarRegistro.valorNeto = entidad.valorNeto;
                        actualizarRegistro.valorIva = entidad.valorIva;
                        actualizarRegistro.valorTotal = entidad.valorTotal;
                        actualizarRegistro.estado = entidad.estado.Value;
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
                        nuevoRegistro.usuarioId = entidad.usuarioId.Value;
                        nuevoRegistro.clientePersonaId = entidad.clientePersonaId;
                        nuevoRegistro.valorNeto = entidad.valorNeto;
                        nuevoRegistro.valorIva = entidad.valorIva;
                        nuevoRegistro.valorTotal = entidad.valorTotal;
                        nuevoRegistro.estado = true;
                        nuevoRegistro.fechaCreacion = DateTime.Now;
                        nuevoRegistro.fechaActualizacion = null;

                        _context.EncabezadoPedidos.Add(nuevoRegistro);
                         await _context.SaveChangesAsync();

                        if (entidad.detallePedidos!=null && entidad.detallePedidos.Count()>0)
                        {
                            foreach (var item in entidad.detallePedidos)
                            {
                                DetallePedido nuevoDetalle = new DetallePedido
                                {
                                    encabezadoPedidosId= nuevoRegistro.id,
                                    productoId = item.productoId,
                                    cantidad = item.cantidad,
                                    porcentajeIva = item.porcentajeIva,
                                    valorUnitario = item.valorUnitario,
                                    estado = true,
                                    fechaCreacion = DateTime.Now,
                                    fechaActualizacion= null
                                };
                                _context.DetallePedidos.Add(nuevoDetalle);
                                ok = await _context.SaveChangesAsync() > 0;
                            }
                        }
                        else
                        {
                            DbTran.Rollback();
                        }
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
                    entidad.usuarioId = data.usuarioId;
                    entidad.clientePersonaId = data.clientePersonaId;
                    entidad.valorNeto = data.valorNeto;
                    entidad.valorIva = data.valorIva;
                    entidad.valorTotal = data.valorTotal;
                    entidad.estado = true;
                    entidad.fechaCreacion = data.fechaCreacion.ToString("yyyy/MM/dd", cultureFecha);
                    entidad.fechaActualizacion = data.fechaActualizacion != null ? data.fechaActualizacion.Value.ToString("yyyy/MM/dd", cultureFecha) : "";

                    var dataDetalle = _context.DetallePedidos.Where(x=>x.encabezadoPedidosId== data.id).ToList();
                    if (dataDetalle!=null)
                    {
                        List<DetallePedidosModel> lisDetalle = new List<DetallePedidosModel>();
                        foreach (var item in dataDetalle)
                        {
                            lisDetalle.Add(new DetallePedidosModel 
                            { 
                                id=item.id,
                                encabezadoPedidosId=item.encabezadoPedidosId,
                                productoId = item.productoId,
                                cantidad = item.cantidad??0,
                                porcentajeIva = item.porcentajeIva??0,
                                valorUnitario = item.valorUnitario ?? 0,
                                fechaCreacion = item.fechaCreacion.ToString("yyyy/MM/dd", cultureFecha),
                            });
                        };
                    }
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
                        var deleteDetalle = _context.DetallePedidos.Where(x => x.encabezadoPedidosId == id).ToList();
                        foreach (DetallePedido item in deleteDetalle)
                        {
                            _context.Entry(item).State = EntityState.Deleted;
                            await _context.SaveChangesAsync();
                        }
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

        public async Task<bool> DeletePedidoDetalle(int id)
        {
            bool ok = false;
            try
            {
                //var deleteDetalle = _context.DetallePedidos.Where(x => x.encabezadoPedidosId == id).FirstOrDefaultAsync();
                //_context.Entry(deleteDetalle).State = EntityState.Deleted;
                //ok = await _context.SaveChangesAsync() > 0;

                string sp = "SpDeleteDetallePedido";
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter() { ParameterName = "@id", Value = id, SqlDbType = SqlDbType.Int });
                var param = parametros.ToArray();
                //ok = await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC {sp} @id={id}") > 0;
                ok = await _context.Database.ExecuteSqlRawAsync($"EXEC {sp} @id", param) > 0;
            }
            catch (Exception e)
            {
                throw;
            }
            return await Task.Run(() => ok);
        }

        public async Task<bool> AgregarDetallePedido(DetallePedidosModel entidad)
        {
            bool ok = false;
            try
            {
                if (entidad.cantidad>0 && entidad.valorUnitario>0)
                {
                    DetallePedido nuevoDetalle = new DetallePedido
                    {
                        encabezadoPedidosId = entidad.encabezadoPedidosId.Value,
                        productoId = entidad.productoId,
                        cantidad = entidad.cantidad,
                        porcentajeIva = entidad.porcentajeIva,
                        valorUnitario = entidad.valorUnitario,
                        estado = true,
                        fechaCreacion = DateTime.Now,
                        fechaActualizacion = null
                    };
                    _context.DetallePedidos.Add(nuevoDetalle);
                    ok = await _context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return await Task.Run(() => ok);
        }

        public async Task<List<DetallePedidosModel>> GetListDetallePedido(int? id)
        {
            List<DetallePedidosModel> lisDetalle = new List<DetallePedidosModel>();
            try
            {
                if (id>0)
                {
                    var dataDetalle = _context.DetallePedidos.Where(x => x.encabezadoPedidosId == id).ToList();
                    if (dataDetalle != null)
                    {
                        foreach (var item in dataDetalle)
                        {
                            lisDetalle.Add(new DetallePedidosModel
                            {
                                id = item.id,
                                encabezadoPedidosId = item.encabezadoPedidosId,
                                productoId = item.productoId,
                                cantidad = item.cantidad ?? 0,
                                porcentajeIva = item.porcentajeIva ?? 0,
                                valorUnitario = item.valorUnitario ?? 0,
                                fechaCreacion = item.fechaCreacion.ToString("yyyy/MM/dd", cultureFecha),
                            });
                        };
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return await Task.Run(() => lisDetalle);
        }
    }
}