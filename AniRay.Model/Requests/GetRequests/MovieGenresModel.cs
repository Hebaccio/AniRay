using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.GetRequests
{
    public class MovieGenresModel
    {
        public int Id { get; set; }
        public virtual Genre Genre { get; set; } = null!;
    }
}
