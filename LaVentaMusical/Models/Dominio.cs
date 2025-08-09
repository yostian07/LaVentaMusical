using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#region Perfiles / Usuarios

[Table("Perfiles")]
public class Perfil
{
    [Key] public int PerfilID { get; set; }
    [Required, StringLength(50)] public string NombrePerfil { get; set; }
}

[Table("Usuarios")]
public class Usuario
{
    [Key] public int UsuarioID { get; set; }

    [Required, StringLength(20)] public string Cedula { get; set; }
    [Required, StringLength(100)] public string NombreCompleto { get; set; }
    [Required, StringLength(10)] public string Genero { get; set; } // 'Masculino'/'Femenino'
    [Required, StringLength(100)] public string CorreoElectronico { get; set; }
    [Required, StringLength(20)] public string TipoTarjeta { get; set; } // VISA/MASTERCARD/AMERICAN EXPRESS
    [Column(TypeName = "decimal")] public decimal DineroDisponible { get; set; }
    [Required, StringLength(19)] public string NumeroTarjeta { get; set; } // 0000-0000-0000-0000

    public int PerfilID { get; set; }
    [ForeignKey(nameof(PerfilID))] public virtual Perfil Perfil { get; set; }

    [StringLength(450)] public string IdentityUserID { get; set; } // FK lógico a AspNetUsers.Id
}

#endregion

#region Géneros / Artistas / Álbumes / Canciones

[Table("Generos")]
public class Genero
{
    [Key, StringLength(10)] public string GeneroID { get; set; }
    [Required, StringLength(100)] public string Descripcion { get; set; }
}

[Table("Artistas")]
public class Artista
{
    [Key, StringLength(10)] public string ArtistaID { get; set; }
    [Required, StringLength(100)] public string NombreArtistico { get; set; }
    public DateTime FechaNacimiento { get; set; }
    [Required, StringLength(100)] public string NombreReal { get; set; }
    [Required, StringLength(50)] public string Nacionalidad { get; set; }
    [StringLength(300)] public string Foto { get; set; }
    [StringLength(300)] public string BiografiaLink { get; set; }
}

[Table("Albumes")]
public class Album
{
    [Key, StringLength(10)] public string AlbumID { get; set; }

    [Required, StringLength(10)] public string ArtistaID { get; set; }
    [ForeignKey(nameof(ArtistaID))] public virtual Artista Artista { get; set; }

    [Required, StringLength(100)] public string NombreAlbum { get; set; }
    public int AnioLanzamiento { get; set; }
    [StringLength(300)] public string Imagen { get; set; }
}

[Table("Canciones")]
public class Cancion
{
    [Key, StringLength(10)] public string CancionID { get; set; }

    [Required, StringLength(10)] public string GeneroID { get; set; }
    [ForeignKey(nameof(GeneroID))] public virtual Genero Genero { get; set; }

    [Required, StringLength(10)] public string AlbumID { get; set; }
    [ForeignKey(nameof(AlbumID))] public virtual Album Album { get; set; }

    [Required, StringLength(100)] public string NombreCancion { get; set; }
    [StringLength(300)] public string LinkMultimedia { get; set; }

    [Column(TypeName = "decimal")] public decimal Precio { get; set; }
    public int CantidadDisponible { get; set; }
}

#endregion

#region Ventas

public enum TipoPago { PayPal, TransferenciaBancaria, TarjetaCredito }

[Table("Ventas")]
public class Venta
{
    [Key] public int VentaID { get; set; }

    public int UsuarioID { get; set; }
    [ForeignKey(nameof(UsuarioID))] public virtual Usuario Usuario { get; set; }

    public DateTime FechaCompra { get; set; }
    [Required, StringLength(50)] public string TipoPago { get; set; } 
    [StringLength(19)] public string NumeroTarjetaEnmascarado { get; set; }
    [StringLength(4)] public string CodigoTarjeta { get; set; } 

    [Column(TypeName = "decimal")] public decimal Subtotal { get; set; }
    [Column(TypeName = "decimal")] public decimal IVA { get; set; }
    [Column(TypeName = "decimal")] public decimal ComisionTarjeta { get; set; }
    [Column(TypeName = "decimal")] public decimal Total { get; set; }
    [Column(TypeName = "decimal")] public decimal DineroUtilizado { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string NumeroFactura { get; set; } 

    public bool EsReversada { get; set; }

    public virtual ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}

[Table("DetalleVenta")]
public class DetalleVenta
{
    [Key] public int DetalleID { get; set; }

    public int VentaID { get; set; }
    [ForeignKey(nameof(VentaID))] public virtual Venta Venta { get; set; }

    [Required, StringLength(10)] public string CancionID { get; set; }
    [ForeignKey(nameof(CancionID))] public virtual Cancion Cancion { get; set; }

    [Column(TypeName = "decimal")] public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    [Column(TypeName = "decimal")] public decimal Subtotal { get; set; }
}

#endregion
