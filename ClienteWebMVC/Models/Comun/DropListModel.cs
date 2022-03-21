namespace ClienteWebMVC.Models.Comun
{
    public class DropListModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public virtual decimal? iva { get; set; }
        public virtual decimal? valor { get; set; }
    }
}
