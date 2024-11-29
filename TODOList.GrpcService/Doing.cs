using System;
using System.Collections.Generic;

namespace TODOList.GrpcService;

public partial class Doing
{
    public string Name { get; set; } = null!;

    public DateTime AddedDate { get; set; }

    public DateTime CompletionDate { get; set; }

    public int Id { get; set; }

    public bool Iscomplete { get; set; }
}
