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
    
    public partial class Contacto
    {
        public int cd_consorcio { get; set; }
        public int cd_contacto { get; set; }
        public string tx_rol { get; set; }
        public string tx_nombre { get; set; }
        public string tx_telefono { get; set; }
    
        public virtual Consorcio Consorcio { get; set; }
    }
}
