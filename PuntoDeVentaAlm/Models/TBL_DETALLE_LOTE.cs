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
    
    public partial class TBL_DETALLE_LOTE
    {
        public int ID_DETALLE { get; set; }
        public Nullable<float> CANTIDAD { get; set; }
        public Nullable<float> VEDIDOS { get; set; }
        public Nullable<System.DateTime> FECHA_REGISTRO { get; set; }
        public Nullable<bool> DISPONIBLE { get; set; }
        public string ID_LOTE { get; set; }
        public string ID_USER_ASP { get; set; }
    
        public virtual TBL_LOTE_PRODUCCION TBL_LOTE_PRODUCCION { get; set; }
    }
}
