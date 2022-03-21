using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiciosNetCore.Configuration;
using ServiciosNetCore.ModelsAPI.Comun;
using ServiciosNetCore.ModelsAPI.DataTable;
using ServiciosNetCore.ModelsAPI.Pedidos;
using ServiciosNetCore.ModelsDB;
using ServiciosNetCore.ModelsDB.Contexts;
using ServiciosNetCore.Repositorio.PedidosES;
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
                        //actualizarRegistro.usuarioId = entidad.usuarioId.Value;
                        actualizarRegistro.clientePersonaId = entidad.clientePersonaId;
                        //actualizarRegistro.valorNeto = entidad.valorNeto;
                        //actualizarRegistro.valorIva = entidad.valorIva;
                        //actualizarRegistro.valorTotal = entidad.valorTotal;
                        actualizarRegistro.estado = entidad.estado;
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

        public async Task<int> CrearPedido(EncabezadoPedidosModel entidad)
        {
            /* Este metodo permite agregar un detalle o varios- depende del frond-end*/
            bool ok = false;
            int result = 0;
            bool detalle = false;

            using (var DbTran = _context.Database.BeginTransaction())
            {
                decimal valorNeto =0;
                decimal valorNetoLinea = 0;
                decimal valorIva = 0;
                decimal valorTotal =0;
                try
                {
                    var verificarExiste = _context.EncabezadoPedidos.Where(x => x.id == entidad.id).FirstOrDefault();

                    if (entidad.detallePedidos != null && entidad.detallePedidos.Count() > 0 && verificarExiste == null) 
                    {
                        foreach (var item in entidad.detallePedidos)
                        {
                            var convertir1 = decimal.TryParse(item.cantidad, NumberStyles.Number, culture, out decimal cantidad);
                            var convertir2 = decimal.TryParse(item.porcentajeIva, NumberStyles.Number, culture, out decimal porcentajeIva);
                            var convertir3 = decimal.TryParse(item.valorUnitario, NumberStyles.Number, culture, out decimal valorUnitario);
           
                            valorNetoLinea = 0;
                            valorNeto += (valorUnitario * cantidad);
                            valorNetoLinea = (valorUnitario * cantidad);
                            valorIva += (valorNetoLinea * porcentajeIva) / 100;
                            valorTotal = (valorNeto + valorIva);
                        }
                        detalle = true;
                    }
                    else
                    {
                        //tiene que existir al menos una linea o detalle y debe ser un pedido nuevo
                        DbTran.Rollback();
                        return await Task.Run(() => result);
                    }
                    EncabezadoPedido nuevoRegistro = new EncabezadoPedido();
                    if (detalle == true)
                    {
                        nuevoRegistro.usuarioId = 1;//entidad.usuarioId.Value; Aqui deb ir el usuario del claims jwt
                        nuevoRegistro.clientePersonaId = entidad.clientePersonaId;
                        nuevoRegistro.valorNeto =Math.Round(valorNeto,2);
                        nuevoRegistro.valorIva = Math.Round(valorIva,2);
                        nuevoRegistro.valorTotal = Math.Round(valorTotal,2);
                        nuevoRegistro.estado = true;
                        nuevoRegistro.fechaCreacion = DateTime.Now;
                        nuevoRegistro.fechaActualizacion = null;

                        _context.EncabezadoPedidos.Add(nuevoRegistro);
                        await _context.SaveChangesAsync();
                        result = nuevoRegistro.id;

                        foreach (var item in entidad.detallePedidos)
                        {
                            var convertir1 = decimal.TryParse(item.cantidad, NumberStyles.Number, culture, out decimal cantidad2);
                            var convertir2 = decimal.TryParse(item.porcentajeIva, NumberStyles.Number, culture, out decimal porcentajeIva2);
                            var convertir3 = decimal.TryParse(item.valorUnitario, NumberStyles.Number, culture, out decimal valorUnitario2);

                            if (convertir1 && convertir2 && convertir3)
                            {
                                DetallePedido nuevoDetalle = new DetallePedido
                                {
                                    encabezadoPedidosId = nuevoRegistro.id,
                                    productoId = item.productoId,
                                    cantidad = cantidad2,
                                    porcentajeIva = porcentajeIva2,
                                    valorUnitario = valorUnitario2,
                                    estado = true,
                                    fechaCreacion = DateTime.Now,
                                    fechaActualizacion = null
                                };
                                _context.DetallePedidos.Add(nuevoDetalle);
                                ok = await _context.SaveChangesAsync()>0;
                            }
                            else
                            {
                                DbTran.Rollback();
                            }
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
            return await Task.Run(() => result);
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
                    entidad.valorNeto = data.valorNeto.ToString("N2",culture);
                    entidad.valorIva = data.valorIva.ToString("N2", culture);
                    entidad.valorTotal = data.valorTotal.ToString("N2", culture);
                    entidad.estado = data.estado.Value;
                    entidad.fechaCreacion = data.fechaCreacion.ToString("yyyy/MM/dd", cultureFecha);
                    entidad.fechaActualizacion = data.fechaActualizacion != null ? data.fechaActualizacion.Value.ToString("yyyy/MM/dd", cultureFecha) : "";

                    var dataDetalle = _context.DetallePedidos.Where(x => x.encabezadoPedidosId == data.id).ToList();
                    if (dataDetalle != null)
                    {
                        List<DetallePedidosModel> lisDetalle = new List<DetallePedidosModel>();
                        foreach (var item in dataDetalle)
                        {
                            lisDetalle.Add(new DetallePedidosModel
                            {
                                id = item.id,
                                encabezadoPedidosId = item.encabezadoPedidosId,
                                productoId = item.productoId,
                                cantidad = item.cantidad.ToString("N2",culture),
                                porcentajeIva = item.porcentajeIva.ToString("N2", culture),
                                valorUnitario = item.valorUnitario.ToString("N2", culture),
                                fechaCreacion = item.fechaCreacion.ToString("yyyy/MM/dd", cultureFecha),
                            });
                        };
                        entidad.detallePedidos = lisDetalle;
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

                var predicado = PredicateBuilder.True<viewEncabezadoPedido>();
                var predicado2 = PredicateBuilder.False<viewEncabezadoPedido>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(dtParameters.search.value))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.cliente.Contains(dtParameters.search.value));
                    predicado2 = predicado2.Or(d => 1 == 1 && d.nombreUsuario.Contains(dtParameters.search.value));
                    predicado2 = predicado2.Or(d => 1 == 1 && d.id.ToString().Contains(dtParameters.search.value));
                    predicado = predicado.And(predicado2);
                }

                datos.recordsFiltered = await _context.viewEncabezadoPedidos.Where(predicado).CountAsync();
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
                    sortcolumn = "PrimerNombre";
                }
                List<viewEncabezadoPedido> datos2 = new List<viewEncabezadoPedido>();
                if (datos.recordsFiltered > 0)
                {
                    datos2 = await _context.viewEncabezadoPedidos.Where(predicado).OrderBy2(sortcolumn, order).Skip(dtParameters.start).Take(dtParameters.length).ToListAsync();
                    datos.data = datos2.Select(x => new viewEncabezadoPedidoModel
                    {
                        id = x.id,
                        usuarioId=x.usuarioId,
                        nombreUsuario=x.nombreUsuario,
                        clientePersonaId=x.clientePersonaId,
                        cliente=x.cliente,
                        valorIva=x.valorIva.ToString("N2",culture),
                        valorNeto=x.valorNeto.ToString("N2",culture),
                        valorTotal=x.valorTotal.ToString("N2",culture),
                        fechaActualizacion = x.fechaActualizacion,
                        fechaCreacion = x.fechaCreacion,
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
            decimal valorNeto = 0;
            decimal valorNetoLinea = 0;
            decimal valorIva = 0;
            decimal valorTotal = 0;
            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var deleteDetalle = await _context.DetallePedidos.Where(x => x.id == id).FirstOrDefaultAsync();
                    _context.Entry(deleteDetalle).State = EntityState.Deleted;
                     await _context.SaveChangesAsync();

                    //string sp = "SpDeleteDetallePedido";
                    //List<SqlParameter> parametros = new List<SqlParameter>();
                    //parametros.Add(new SqlParameter() { ParameterName = "@id", Value = id, SqlDbType = SqlDbType.Int });
                    //var param = parametros.ToArray();
                    ////ok = await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC {sp} @id={id}") > 0;
                    // await _context.Database.ExecuteSqlRawAsync($"EXEC {sp} @id", param);

                    var detallePedidos = await _context.DetallePedidos.Where(x => x.encabezadoPedidosId == deleteDetalle.encabezadoPedidosId).ToListAsync();

                    if (detallePedidos != null && detallePedidos.Count() > 0)
                    {
                        foreach (var item in detallePedidos)
                        {
                            valorNetoLinea = 0;
                            valorNeto += (item.valorUnitario * item.cantidad);
                            valorNetoLinea = (item.valorUnitario * item.cantidad);
                            valorIva += (valorNetoLinea * item.porcentajeIva) / 100;
                            valorTotal = (valorNeto + valorIva);
                        }
                    }
                    var encabezadoPedidos = await _context.EncabezadoPedidos.Where(x => x.id == deleteDetalle.encabezadoPedidosId).FirstOrDefaultAsync();
                    if (encabezadoPedidos!=null)
                    {
                        encabezadoPedidos.valorNeto =Math.Round(valorNeto,2);
                        encabezadoPedidos.valorIva = Math.Round(valorIva,2);
                        encabezadoPedidos.valorTotal = Math.Round(valorTotal,2);
                        _context.Entry(encabezadoPedidos).State = EntityState.Modified;
                        ok = await _context.SaveChangesAsync() > 0;
                    }
                    else if (encabezadoPedidos != null &&(detallePedidos == null || detallePedidos.Count() == 0))
                    {
                        //podria eliminar el pedido si ya no tiene detalle
                        //_context.Entry(encabezadoPedidos).State = EntityState.Deleted;
                        //ok = await _context.SaveChangesAsync() > 0;
                    }
                    if (ok)
                    {
                        DbTran.Commit();
                    }
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return await Task.Run(() => ok);
        }

        public async Task<bool> AgregarDetallePedido(DetallePedidosModel entidad)
        {
            bool ok = false;
            decimal valorNeto = 0;
            decimal valorNetoLinea = 0;
            decimal valorIva = 0;
            decimal valorTotal = 0;
            var convertir1 = decimal.TryParse(entidad.cantidad, NumberStyles.Number, culture, out decimal cantidad);
            var convertir2 = decimal.TryParse(entidad.porcentajeIva, NumberStyles.Number, culture, out decimal porcentajeIva);
            var convertir3 = decimal.TryParse(entidad.valorUnitario, NumberStyles.Number, culture, out decimal valorUnitario);
            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    if (convertir1 && convertir2 && convertir3)
                    {
                        DetallePedido nuevoDetalle = new DetallePedido
                        {
                            encabezadoPedidosId = entidad.encabezadoPedidosId.Value,
                            productoId = entidad.productoId,
                            cantidad = cantidad,
                            porcentajeIva = porcentajeIva,
                            valorUnitario = valorUnitario,
                            estado = true,
                            fechaCreacion = DateTime.Now,
                            fechaActualizacion = null
                        };
                        _context.DetallePedidos.Add(nuevoDetalle);
                        await _context.SaveChangesAsync();

                        var detallePedidos =await _context.DetallePedidos.Where(x => x.encabezadoPedidosId == entidad.encabezadoPedidosId.Value).ToListAsync();

                        if (detallePedidos != null && detallePedidos.Count() > 0)
                        {
                            foreach (var item in detallePedidos)
                            {
                                valorNetoLinea = 0;
                                valorNeto += (item.valorUnitario * item.cantidad);
                                valorNetoLinea = (item.valorUnitario * item.cantidad);
                                valorIva += (valorNetoLinea * item.porcentajeIva) / 100;
                                valorTotal = (valorNeto + valorIva);
                            }

                            var encabezadoPedidos = await _context.EncabezadoPedidos.Where(x => x.id == entidad.encabezadoPedidosId.Value).FirstOrDefaultAsync();
                            encabezadoPedidos.valorNeto = Math.Round(valorNeto,2);
                            encabezadoPedidos.valorIva = Math.Round(valorIva,2);
                            encabezadoPedidos.valorTotal = Math.Round(valorTotal,2);
                            _context.Entry(encabezadoPedidos).State = EntityState.Modified;
                            ok = await _context.SaveChangesAsync() > 0;
                        }

                        if (ok)
                        {
                            DbTran.Commit();
                        }
                    }
                }
                catch (Exception)
                {
                    DbTran.Rollback();
                    throw;
                }
            }
            return await Task.Run(() => ok);
        }

        public async Task<List<DetallePedidosModel>> GetListDetallePedido(int? id)
        {
            List<DetallePedidosModel> lisDetalle = new List<DetallePedidosModel>();
            try
            {
                if (id > 0)
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
                                cantidad = item.cantidad.ToString("N2", culture),
                                porcentajeIva = item.porcentajeIva.ToString("N2", culture),
                                valorUnitario = item.valorUnitario.ToString("N2", culture),
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