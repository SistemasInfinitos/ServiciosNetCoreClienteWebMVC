using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiciosNetCore.Configuration;
using ServiciosNetCore.ModelsAPI.Comun;
using ServiciosNetCore.ModelsAPI.DataTable;
using ServiciosNetCore.ModelsAPI.Productos;
using ServiciosNetCore.ModelsDB;
using ServiciosNetCore.ModelsDB.Contexts;
using ServiciosNetCore.Repositorio.ProductosES;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ServiciosNetCore.Repositorio.ProcuctosES
{
    public class ProductosESRepositorio : IProductosESRepositorio
    {
        private readonly JwtConfiguracion _jwtConfig;
        private readonly Context _context;
        private readonly AuthorizationHandlerContext _HandlerContext;

        public ProductosESRepositorio(IOptionsMonitor<JwtConfiguracion> optionsMonitor, Context context, AuthorizationHandlerContext HandlerContext)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _context = context;
            _HandlerContext = HandlerContext;
        }

        private readonly CultureInfo culture = new CultureInfo("is-IS");
        private readonly CultureInfo cultureFecha = new CultureInfo("en-US");

        public async Task<bool> ActualizarProducto(ProductosModel entidad)
        {
            bool ok = false;
            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    Producto actualizarRegistro = _context.Productos.Where(x => x.id == entidad.id).FirstOrDefault();
                    var convertir = decimal.TryParse(entidad.valorUnitario, NumberStyles.Number, culture, out decimal valorUnitario);
                    var convertirIva = decimal.TryParse(entidad.iva, NumberStyles.Number, culture, out decimal iva);
                    if (actualizarRegistro != null && convertir)
                    {
                        actualizarRegistro.descripcion = entidad.descripcion;
                        actualizarRegistro.valorUnitario = valorUnitario;
                        actualizarRegistro.iva = iva;
                        actualizarRegistro.estado = entidad.estado;
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

        public async Task<bool> CrearProducto(ProductosModel entidad)
        {
            bool ok = false;

            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var verificarExiste = _context.Productos.Where(x => x.descripcion == entidad.descripcion).FirstOrDefault();
                    Producto nuevoRegistro = new Producto();
                    var convertir = decimal.TryParse(entidad.valorUnitario, NumberStyles.Number, culture, out decimal valorUnitario);
                    var convertirIva = decimal.TryParse(entidad.iva, NumberStyles.Number, culture, out decimal iva);

                    if (verificarExiste == null && convertir)
                    {
                        nuevoRegistro.descripcion = entidad.descripcion;
                        nuevoRegistro.valorUnitario = valorUnitario;
                        nuevoRegistro.iva = iva;
                        nuevoRegistro.estado = true;
                        nuevoRegistro.fechaCreacion = DateTime.Now;
                        nuevoRegistro.fechaActualizacion = null;

                        _context.Productos.Add(nuevoRegistro);
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

        public async Task<ProductosModel> GetProducto(string buscar, int? Id)
        {
            ProductosModel persona = new ProductosModel();
            try
            {
                var predicado = PredicateBuilder.True<Producto>();
                var predicado2 = PredicateBuilder.False<Producto>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.descripcion.Contains(buscar));
                    predicado = predicado.And(predicado2);
                }
                if (Id != null)
                {
                    predicado = predicado.And(x => x.id == Id);
                }
                var data = _context.Productos.Where(predicado).FirstOrDefault();
                if (data != null)
                {
                    persona.id = data.id;
                    persona.descripcion = data.descripcion;
                    persona.valorUnitario = data.valorUnitario.ToString("N2", culture);
                    persona.iva = data.iva.ToString("N2", culture);
                    persona.estado = data.estado ?? false;
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

        public async Task<DataTableResponseProducto> GetProductosDataTable(DataTableParameter dtParameters)
        {
            try
            {
                DataTableResponseProducto datos = new DataTableResponseProducto();
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

                var predicado = PredicateBuilder.True<Producto>();
                var predicado2 = PredicateBuilder.False<Producto>();
                //predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(dtParameters.search.value))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.descripcion.Contains(dtParameters.search.value));
                    predicado = predicado.And(predicado2);
                }

                datos.recordsFiltered = _context.Productos.Where(predicado).ToList().Count();
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
                List<Producto> datos2 = new List<Producto>();
                if (datos.recordsFiltered > 0)
                {
                    datos2 = _context.Productos.Where(predicado).OrderBy2(sortcolumn, order).Skip(dtParameters.start).Take(dtParameters.length).ToList();
                    datos.data = datos2.Select(x => new ProductosModel
                    {
                        id = x.id,
                        descripcion = x.descripcion,
                        valorUnitario = x.valorUnitario.ToString("N2", culture),
                        iva = x.iva.ToString("N2", culture),
                        estado = x.estado ?? false,
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

        public async Task<bool> DeleteProducto(int id)
        {
            bool ok = false;
            using (var DbTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var delete = _context.Productos.Where(x => x.id == id).FirstOrDefault();

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

        public async Task<List<DropListModel>> GetProductosDropList(string buscar, int? id)
        {
            List<DropListModel> datos = new List<DropListModel>();
            try
            {
                var predicado = PredicateBuilder.True<Producto>();
                var predicado2 = PredicateBuilder.False<Producto>();
                predicado = predicado.And(d => d.estado == true);

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    predicado2 = predicado2.Or(d => 1 == 1 && d.descripcion.Contains(buscar));
                    predicado = predicado.And(predicado2);
                }
                if (id != null)
                {
                    predicado = predicado.And(d => d.id == id);
                }

                var data = _context.Productos.Where(predicado).Take(10).ToList();

                datos = data.Select(x => new DropListModel
                {
                    id = x.id,
                    text = x.descripcion,
                    iva = x.iva.ToString("N2", culture),
                    valor = x.valorUnitario.ToString("N2", culture),
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