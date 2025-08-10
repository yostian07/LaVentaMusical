using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaVentaMusical.Models
{
    #region Perfiles / Usuarios

    [Table("Perfiles")]
    public class Perfil
    {
        [Key] public int PerfilID { get; set; }
        [Required, StringLength(50)] public string NombrePerfil { get; set; }
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

    #endregion
}
