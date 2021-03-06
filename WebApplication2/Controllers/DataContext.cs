﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication2.Models;

namespace Inmobiliaria_.Net_Core.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Inmueble> Inmuebles { get; set; }
        public DbSet<FotoPerfil> FotoPerfil { get; set; }
        public DbSet<TipoInmueble> TipoInmueble { get; set; }
        public DbSet<Galeria> Galeria { get; set; }
    }
}
