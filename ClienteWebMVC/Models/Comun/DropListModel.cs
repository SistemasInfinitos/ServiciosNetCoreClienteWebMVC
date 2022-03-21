namespace ClienteWebMVC.Models.Comun
{
    public class DropListModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public virtual string iva { get; set; }
        public virtual string valor { get; set; }
    }
}
