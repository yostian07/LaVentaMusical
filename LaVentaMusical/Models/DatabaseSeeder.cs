using System;
using System.Linq;

namespace LaVentaMusical.Models
{
    public static class DatabaseSeeder
    {
        public static void SeedDatabase()
        {
            using (var db = new PAV_PF_Grupo02Entities())
            {
                // Solo sembrar si la base de datos está vacía
                if (db.Generos.Any()) return;

                // Sembrar perfiles (solo una vez)
                var perfiles = new[]
                {
                    new Perfil { NombrePerfil = "administrador" },
                    new Perfil { NombrePerfil = "usuario" },
                    new Perfil { NombrePerfil = "contabilidad" }
                };
                db.Perfiles.AddRange(perfiles);
                db.SaveChanges();

                // Sembrar usuario demo
                var usuarioDemo = new Usuario
                {
                    Cedula = "123456789",
                    NombreCompleto = "Usuario Demo",
                    Genero = "Masculino",
                    CorreoElectronico = "demo@laventamusical.com",
                    TipoTarjeta = "VISA",
                    DineroDisponible = 15000m,
                    NumeroTarjeta = "4111-1111-1111-1111",
                    PerfilID = perfiles.First(p => p.NombrePerfil == "usuario").PerfilID
                };
                db.Usuarios.Add(usuarioDemo);
                db.SaveChanges();

                // Sembrar géneros
                var generos = new[]
                {
                    new Genero { GeneroID = "POP", Descripcion = "Pop" },
                    new Genero { GeneroID = "ROCK", Descripcion = "Rock" },
                    new Genero { GeneroID = "JAZZ", Descripcion = "Jazz" },
                    new Genero { GeneroID = "REGGAE", Descripcion = "Reggaeton" },
                    new Genero { GeneroID = "BLUES", Descripcion = "Blues" }
                };
                db.Generos.AddRange(generos);
                db.SaveChanges();

                // Sembrar artistas
                var artistas = new[]
                {
                    new Artista
                    {
                        ArtistaID = "ART001",
                        NombreArtistico = "Los Ángeles Azules",
                        NombreReal = "Grupo Los Ángeles Azules",
                        FechaNacimiento = new DateTime(1976, 1, 1),
                        Nacionalidad = "México",
                        Foto = "/Content/images/artistas/angeles-azules.jpg",
                        BiografiaLink = "https://es.wikipedia.org/wiki/Los_Ángeles_Azules"
                    },
                    new Artista
                    {
                        ArtistaID = "ART002",
                        NombreArtistico = "Jesse & Joy",
                        NombreReal = "Jesse Eduardo Huerta y Tirzah Joy Huerta",
                        FechaNacimiento = new DateTime(1982, 4, 12),
                        Nacionalidad = "México",
                        Foto = "/Content/images/artistas/jesse-joy.jpg",
                        BiografiaLink = "https://es.wikipedia.org/wiki/Jesse_%26_Joy"
                    },
                    new Artista
                    {
                        ArtistaID = "ART003",
                        NombreArtistico = "Manu Chao",
                        NombreReal = "José Manuel Arturo Tomás Chao",
                        FechaNacimiento = new DateTime(1961, 6, 21),
                        Nacionalidad = "Francia",
                        Foto = "/Content/images/artistas/manu-chao.jpg",
                        BiografiaLink = "https://es.wikipedia.org/wiki/Manu_Chao"
                    }
                };
                db.Artistas.AddRange(artistas);
                db.SaveChanges();

                // Sembrar álbumes
                var albumes = new[]
                {
                    new Album
                    {
                        AlbumID = "ALB001",
                        ArtistaID = "ART001",
                        NombreAlbum = "Cómo Te Voy a Olvidar",
                        AnioLanzamiento = 1996,
                        Imagen = "/Content/images/albumes/como-te-voy-a-olvidar.jpg"
                    },
                    new Album
                    {
                        AlbumID = "ALB002",
                        ArtistaID = "ART002",
                        NombreAlbum = "Esta Es Mi Vida",
                        AnioLanzamiento = 2006,
                        Imagen = "/Content/images/albumes/esta-es-mi-vida.jpg"
                    },
                    new Album
                    {
                        AlbumID = "ALB003",
                        ArtistaID = "ART003",
                        NombreAlbum = "Clandestino",
                        AnioLanzamiento = 1998,
                        Imagen = "/Content/images/albumes/clandestino.jpg"
                    }
                };
                db.Albumes.AddRange(albumes);
                db.SaveChanges();

                // Sembrar canciones
                var canciones = new[]
                {
                    new Cancion
                    {
                        CancionID = "C-001",
                        GeneroID = "POP",
                        AlbumID = "ALB001",
                        NombreCancion = "Cómo Te Voy a Olvidar",
                        LinkMultimedia = "https://youtube.com/watch?v=example1",
                        Precio = 1200m,
                        CantidadDisponible = 100
                    },
                    new Cancion
                    {
                        CancionID = "C-002",
                        GeneroID = "POP",
                        AlbumID = "ALB001",
                        NombreCancion = "El Listón de Tu Pelo",
                        LinkMultimedia = "https://youtube.com/watch?v=example2",
                        Precio = 1200m,
                        CantidadDisponible = 75
                    },
                    new Cancion
                    {
                        CancionID = "C-003",
                        GeneroID = "POP",
                        AlbumID = "ALB002",
                        NombreCancion = "Espacio Sideral",
                        LinkMultimedia = "https://youtube.com/watch?v=example3",
                        Precio = 1500m,
                        CantidadDisponible = 50
                    },
                    new Cancion
                    {
                        CancionID = "C-004",
                        GeneroID = "REGGAE",
                        AlbumID = "ALB003",
                        NombreCancion = "Bongo Bong",
                        LinkMultimedia = "https://youtube.com/watch?v=example4",
                        Precio = 1800m,
                        CantidadDisponible = 25
                    },
                    new Cancion
                    {
                        CancionID = "C-005",
                        GeneroID = "REGGAE",
                        AlbumID = "ALB003",
                        NombreCancion = "Clandestino",
                        LinkMultimedia = "https://youtube.com/watch?v=example5",
                        Precio = 1800m,
                        CantidadDisponible = 30
                    }
                };
                db.Canciones.AddRange(canciones);
                db.SaveChanges();
            }
        }
    }
}