namespace Agency.Economy.Model
{
    public class Offer
    {
        public IResource Resource { get; set; }
        public INode Location { get; set; }
        public int Quantity { get; set; }
        public float Cost { get; set; }
    }
}