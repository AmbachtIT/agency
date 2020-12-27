namespace Agency.Economy.Model
{
    public interface IEconomy
    {

        void AddSupply(INode node, IResource resource, int amount);
        
        void AddDemand(INode node, IResource resource, int amount);

    }
}