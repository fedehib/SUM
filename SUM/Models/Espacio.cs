//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SUM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Espacio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Espacio()
        {
            this.Reserva = new HashSet<Reserva>();
        }
    
        public int cd_consorcio { get; set; }
        public int cd_espacio { get; set; }
        public string tx_descripcion { get; set; }
        public double fl_costo_semana { get; set; }
        public double fl_costo_fin_de_semana { get; set; }
        public double fl_limpieza { get; set; }
        public double fl_multa { get; set; }
    
        public virtual Consorcio Consorcio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reserva> Reserva { get; set; }
    }
}
