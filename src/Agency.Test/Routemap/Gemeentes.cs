using System;
using System.Collections.Generic;
using System.Linq;
using Agency.Network.RoadRunner;
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


        private void PlotGemeente(Func<RouteMap, (int, int)> getValue, string title, string filename)
        {
            var plot = new PlotModel()
            {
                Title = title
            };
            var seriesByIndex = new Dictionary<int, ScatterSeries>();
            
            var source = new GemeenteRouteMapDatasource(@"D:\Data\OpenFietsModel\Gemeentes");
            var byCount = new Dictionary<string, int>();
            foreach (var gemeente in source.Gemeentes())
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
                    byCount.Add(gemeente.Name, x);
                }
            }
            var exporter = new PngExporter()
            {
                Width = 900,
                Height = 600,
                Background = OxyColors.White
            };
            exporter.ExportToFile(plot, @"C:\Projects\Agency\doc\performance\" + filename);

            foreach (var kv in byCount.OrderByDescending(k => k.Value))
            {
                TestContext.WriteLine($"{kv.Key}: {kv.Value}");
            }
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
            "Den Haag",
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
        };

        private static IEnumerable<Gemeente> AllGemeentes()
        {
            var source = new GemeenteRouteMapDatasource(@"D:\Data\OpenFietsModel\Gemeentes");
            foreach (var gemeente in source.Gemeentes().OrderBy(g => g.Code))
            {
                yield return gemeente;
            }
        }
    }
}