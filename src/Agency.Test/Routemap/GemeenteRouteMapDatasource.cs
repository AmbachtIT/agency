using System.Collections.Generic;
using System.IO;
using System.Linq;
using Agency.Network.RoadRunner;

namespace Agency.Test.Routemap
{
    public class GemeenteRouteMapDatasource
    {

        public GemeenteRouteMapDatasource(string path)
        {
            this.path = path;
        }
        
        private readonly string path;


        public IEnumerable<Gemeente> Gemeentes()
        {
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                var info = new DirectoryInfo(dir);
                yield return new Gemeente()
                {
                    Code = info.Name,
                    Name = GetName(dir)
                };
            }
        }

        public RouteMap LoadMap(Gemeente gemeente)
        {
            var routeMapPath = Path.Combine(this.path, gemeente.Code, "Routemap.bin");
            if (File.Exists(routeMapPath))
            {
                return RouteMap.LoadBinary(routeMapPath);
            }

            return null;
        }

        private string GetName(string dir)
        {
            foreach (var file in Directory.GetFiles(dir, "*.txt"))
            {
                var info = new FileInfo(file);
                if (info.Name.ToLower() != "stationexits.txt")
                {
                    return info.Name.Replace(".txt", "");
                }
            }

            return null;
        }
    }

    public class Gemeente
    {
        public string Code { get; set; }

        public string Name { get; set; }

    }
}