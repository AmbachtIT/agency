using System;
using System.Collections.Generic;
using System.Linq;
using Agency.Common;
using Agency.Network.RoadRunner;
using Agency.Test.Extensibility;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using LinearColorAxis = OxyPlot.Axes.LinearColorAxis;
using ScatterSeries = OxyPlot.Series.ScatterSeries;

namespace Agency.Test.Routemap
{
    public class Gemeentes
    {
        [Test(), Explicit()]
        public void EnumGemeentes()
        {
            foreach (var gemeente in AllGemeentes())
            {
                TestContext.WriteLine($"{gemeente.Code} {gemeente.Name}");
            }
        }
        
        [Test(), Explicit()]
        public void PlotVerticesVersusEdgesOriginal()
        {
            PlotGemeente(map => (map.Vertices.Count, map.Edges.Count), "Graph nodes versus edges", "nodes-vs-edges.png");
        }


        [Test(), Explicit()]
        public void PlotVerticesVersusEdgesModalities()
        {
            foreach (var modality in new[]
            {
                    Modality.Car,
                    Modality.Bike,
                    Modality.Walk, 
                })
            {
                PlotVerticesVersusEdgesModality(modality);
            }
        }
        
        private void PlotVerticesVersusEdgesModality(Modality modality)
        {
            var name = modality.Name.ToLower();
            PlotGemeente(map =>
            {
                var network = map.CreateNetwork(modality);
                return (network.Nodes.Count, network.Edges.Count);
            }, $"Graph nodes versus edges (modality: {name})", $"nodes-vs-edges-{name}.png");
        }


        private void PlotGemeente(Func<RouteMap, (int, int)> getValue, string title, string filename, Func<Gemeente, bool> shouldInclude = null)
        {
            shouldInclude ??= g => true;
            
            var plot = new PlotModel()
            {
                Title = title
            };
            var seriesByIndex = new Dictionary<int, ScatterSeries>();
            
            var source = new GemeenteRouteMapDatasource(@"D:\Data\OpenFietsModel\Gemeentes");
            foreach (var gemeente in source.Gemeentes().Where(shouldInclude))
            {
                var map = source.LoadMap(gemeente);
                if (map != null)
                {
                    var index = GetColorIndex(gemeente);
                    if (!seriesByIndex.TryGetValue(index, out var series))
                    {
                        seriesByIndex.Add(index, series = new ScatterSeries()
                        {
                            Title = index == 63 ? "Rest" : gemeente.Name,
                            MarkerType = MarkerType.Square,
                            MarkerFill = OxyPalettes.Hue64.Colors[index]
                        });
                        plot.Series.Add(series);
                    }

                    var (x, y) = getValue(map);
                    series.Points.Add(new ScatterPoint(x, y, 3));
                }
            }
            var exporter = new PngExporter()
            {
                Width = 900,
                Height = 600,
                Background = OxyColors.White
            };
            exporter.ExportToFile(plot, @"C:\Projects\Agency\doc\performance\" + filename);
        }

        private int GetColorIndex(Gemeente gemeente)
        {
            var index = importantGemeentes.IndexOf(gemeente.Name);
            if (index == -1)
            {
                return 63;
            }

            return index;
        }

        private static readonly List<string> importantGemeentes = new List<string>()
        {
            "Amsterdam",
            "Rotterdam",
            "'s-Gravenhage",
            "Eindhoven",
            "Tilburg",
            "Groningen",
            "Almere",
            "Breda",
            "Nijmegen",
            "Apeldoorn",
            "Haarlem",
            "Arnhem",
            "Enschede",
            "Haarlemmermeer",
            "Amersfoort",
            "Zaanstad",
            "Den Bosch",
            "Zwolle",
            "Zoetermeer",
            "Leeuwarden",
            "Leiden",
            "Maastricht",
            "Dordrecht",
            "Ede",
            "Noordoostpolder",
            "Twenterand",
            "Tiel"
        };

        [Test(), Explicit()]
        public void Benchmark()
        {
            var checksum = new Checksum();
            PlotGemeente(map =>
            {
                var network = map.CreateNetwork(Modality.Car);
                var benchmark = new Benchmark()
                {
                    Network = network
                };
                var result = benchmark.Run();
                checksum.Add(result.Checksum);
                return (network.Nodes.Count, (int)result.Stats.Average);
            }, $"Average pathfinding duration", $"duration.png", g => importantGemeentes.Contains(g.Name));
            TestContext.WriteLine($"Checksum: {checksum}");
            Assert.AreEqual(38616, checksum.Value);
        }

        private IEnumerable<Gemeente> AllGemeentes()
        {
            var source = new GemeenteRouteMapDatasource(@"D:\Data\OpenFietsModel\Gemeentes");
            foreach (var gemeente in source.Gemeentes().OrderBy(g => g.Code))
            {
                yield return gemeente;
            }
        }
    }
}