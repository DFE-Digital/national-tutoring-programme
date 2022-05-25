﻿namespace Domain.Search;

public class TuitionPartnerSearchResult
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Website { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Subject[] Subjects { get; set; } = null!;
    public TuitionType[] TuitionTypes { get; set; } = null!;
}