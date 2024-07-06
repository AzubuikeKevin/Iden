namespace Iden.Entities
{
    public record ValidationError
    {
        public string field { get; set; }
        public string message { get; set; }
    }


}
