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
    
    public partial class TBL_INVENTARIOS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TBL_INVENTARIOS()
        {
            this.TBL_DETALLES_INVENT = new HashSet<TBL_DETALLES_INVENT>();
        }
    
        public int ID_INVENTARIO { get; set; }
        public Nullable<System.DateTime> FECHA { get; set; }
        public string OBSERVACIONES { get; set; }
        public Nullable<bool> ESTATUS { get; set; }
        public int ID_ALMACEN { get; set; }
        public string ID_USER_ASP { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_DETALLES_INVENT> TBL_DETALLES_INVENT { get; set; }
        public virtual TBL_ALMACENES TBL_ALMACENES { get; set; }
    }
}
