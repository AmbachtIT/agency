using Agency.Network.RoadRunner;

namespace Agency
{
    public class AgencyHost
    {
        public AgencyHost()
        {
            Map = RouteMap.LoadBinary(@"D:\Data\OpenFietsModel\Gemeentes\0344\RouteMap.bin");
        }
        
        public RouteMap Map { get; }
    }
}