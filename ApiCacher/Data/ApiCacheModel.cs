namespace ApiCacher.Data;

public class ApiCacheModel
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}
