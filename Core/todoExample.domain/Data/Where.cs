namespace todoExample.domain.Data;

public class Where
{
    public string Field { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? Value2 { get; set; }
    public string? Op { get; set; }
    public string? AndOr { get; set; }
}