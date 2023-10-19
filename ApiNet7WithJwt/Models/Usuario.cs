using System;
using System.Collections.Generic;

namespace ApiNet7WithJwt.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Contrasena { get; set; }

    public virtual ICollection<HistorialRefreshToken> HistorialRefreshTokens { get; set; } = new List<HistorialRefreshToken>();
}
