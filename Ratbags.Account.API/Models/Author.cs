using System;
using System.Collections.Generic;

namespace Ratbags.Login.Models;

public partial class Author
{
    public Guid AuthorId { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
