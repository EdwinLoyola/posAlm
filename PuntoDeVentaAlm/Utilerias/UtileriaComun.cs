using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PuntoDeVentaAlm.Models;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace PuntoDeVentaAlm.Utilerias
{
    public class UtileriaComun : Controller
    {
        public string idProducto(string descripcion)
        { 
            using(ALMENDRITAEntities db = new ALMENDRITAEntities())
            {
                return db.TBL_PRODUCTOS.Where(x => x.DESCRIPCION.Equals(descripcion)).Select(x => x.ID_PRODUCTO).ToString();
            }
        }

        public JsonResult BuscarProducto(string term)
        {
            using (ALMENDRITAEntities db = new ALMENDRITAEntities())
            {
                var resultado = db.TBL_PRODUCTOS.Where(x => x.DESCRIPCION.Contains(term)).Where(x => x.ESTATUS == true).Select(x => x.DESCRIPCION).Take(10).ToList();
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }

        public string GeneraIdProduccion()
        {
            string idP = "LOT";
            int num;
            using(ALMENDRITAEntities db = new ALMENDRITAEntities())
            {
                num=db.TBL_LOTE_PRODUCCION.Count();

                if(num<9)
                {
                    idP += "0000" + (num + 1);
                }
                else if(num<99 && num>8)
                {
                    idP += "000" + (num + 1);
                }
                else if(num <999 && num >98)
                {
                    idP += "00" + (num + 1);
                }
                else if(num<9999 && num > 998)
                {
                    idP += "0" + (num + 1);
                }
                else if (num < 99999 && num > 9998)
                {
                    idP += (num + 1);
                }
                else
                {
                    idP = "Error al generar ID para lote de produccion nuevo, contacte al administrador.";
                }

                return idP;

            }
        }

        public void qrLote(string mensaje, string id)
        {
            try
            {
                string ruta = @"C:\Users\Sistemas\Pictures\qrLotes\" + id + ".png";
                //string ruta1 = Server.MapPath("~");

                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode = new QrCode();

                qrEncoder.TryEncode(mensaje, out qrCode);
                GraphicsRenderer render = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero), Brushes.Black, Brushes.White);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                render.WriteToStream(qrCode.Matrix, System.Drawing.Imaging.ImageFormat.Png, ms);
                var imageTemporal = new System.Drawing.Bitmap(ms);
                var imagen = new System.Drawing.Bitmap(imageTemporal, new Size(new Point(100, 100)));

                imagen.Save(ruta, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception e)
            { throw; }
        }

        public bool bajaStock(string idProducto,float? cantidad, int idAlmacen)
        {
            using (ALMENDRITAEntities db = new ALMENDRITAEntities())
            {
                try
                {
                    var bStock = db.TBL_STOCK.Where(x => x.ID_ALMAC == idAlmacen && x.ID_PRODUC.Equals(idProducto)).ToList();
                    bStock[0].STOCK -= cantidad;
                    if (bStock[0].STOCK >= 0)
                    {
                        db.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public string obtieneIdProduct(string producto)
        {
            string[] id = producto.Split('|');
            string i = id[id.Length - 1];
            return i;
        }

        
        public int obtieneIdInvent(int idAlm)
        {
            int idInventario;
            using (ALMENDRITAEntities db = new ALMENDRITAEntities())
            {
                var inventActivo = db.TBL_INVENTARIOS.AsNoTracking().Where(x => x.ID_ALMACEN == idAlm && x.ESTATUS == true).ToList();
                idInventario = inventActivo[0].ID_INVENTARIO;
            }

            return idInventario;
        }

        public bool existeProducto(string idProduc)
        {
            bool ret = false;
            using (ALMENDRITAEntities db = new ALMENDRITAEntities())
            {
                var producto = db.TBL_PRODUCTOS.AsNoTracking().Where(x => x.ID_PRODUCTO.Equals(idProduc) && x.ESTATUS == true).Select(x => x.ESTATUS).ToArray();
                if (producto.Count() == 1)
                    ret = true;
            }
            return ret;
        }



    }
}