//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PuntoDeVentaAlm.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBL_STOCK
    {
        public int ID_STOCK { get; set; }
        public Nullable<float> STOCK { get; set; }
        public Nullable<float> PEDIDO { get; set; }
        public Nullable<float> COMPROMETIDO { get; set; }
        public string ID_PRODUC { get; set; }
        public int ID_ALMAC { get; set; }
        public Nullable<System.DateTime> ULTIMA_FECHA_ACTUALIZACION { get; set; }
    
        public virtual TBL_PRODUCTOS TBL_PRODUCTOS { get; set; }
        public virtual TBL_ALMACENES TBL_ALMACENES { get; set; }
    }
}