using BingMapsRESTToolkit;
using eMapy.Models;
using eMapy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;

namespace eMapy.BusinessLogic
{
    using System.Threading.Tasks;

    public class GlobalMethods
    {
        public static string ConnectionString = string.Empty;
        public static string key = string.Empty;
        public static Route route;
        public static int counter;
        public static string FilePathString = string.Empty;

        public static string GetPathXmlFile()
        {
            return FilePathString; // Environment.SpecialFolder.CommonDocuments + "\\Sage\\kh.xml";
        }

        public static Route GetRoute(List<SimpleWaypoint> waypoints)
        {
            var routeResult = new Route();
            var request = new RouteRequest
            {
                BingMapsKey = MainWindowViewModel.CredentialKey,
                Culture = "pl-PL",
                Waypoints = waypoints,
                RouteOptions = new RouteOptions
                {
                    DistanceUnits = DistanceUnitType.Kilometers,
                    TravelMode = TravelModeType.Driving,
                    Optimize = RouteOptimizationType.TimeAvoidClosure,
                    RouteAttributes = new List<RouteAttributeType>
                    {
                        RouteAttributeType.RoutePath
                    }
                }
            };
            Uri requestUri = new Uri(request.GetRequestUrl());
            GetResponse(requestUri, x =>
            {
                routeResult = x.ResourceSets[0].Resources[0] as Route;
            });
            counter++;
            return routeResult;
        }

        public static void GetResponse(Uri uri, Action<Response> callback)
        {
            if (callback != null)
            {
                WebClient wc = new WebClient();
                using (var s = wc.OpenRead(uri))
                {
                    var ser = new DataContractJsonSerializer(typeof(Response));
                    callback(ser.ReadObject(s) as Response);
                }
            }
        }

        public static List<SimpleWaypoint> ConvertEMapyPointToSimpleWaypoints(List<EmapyPoint> eMapyPoints)
        {
            var result = new List<SimpleWaypoint>();

            foreach (var item in eMapyPoints)
            {
                var newItem = new SimpleWaypoint
                {
                    Address = item.MapyAdress,
                    Coordinate = new Coordinate
                    {
                        Latitude = item.Location.Latitude,
                        Longitude = item.Location.Longitude
                    }
                };
                result.Add(newItem);
            }
            return result;
        }

        public static async Task<Location> GetAdressByCoordinatesAsync(Coordinate point)
        {
            var Result = new Location();
            var request = new ReverseGeocodeRequest
            {
                BingMapsKey = MainWindowViewModel.CredentialKey,
                Culture = "pl-PL",
                Point = point
            };
            Uri requestUri = new Uri(request.GetRequestUrl());
            GetResponse(requestUri, x =>
            {
                Result = x.ResourceSets[0].Resources[0] as Location;
            });
            counter++;
            return Result;
        }

        public static async Task<Location> GetLocationAsync(string mapyAdress)
        {
            Location result = null;
            try
            {
                GeocodeRequest geocodeRequest = new GeocodeRequest
                {
                    BingMapsKey = MainWindowViewModel.CredentialKey,
                    Culture = "pl-PL",
                    Query = mapyAdress,
                    IncludeNeighborhood = false
                };

                Uri RequestUri = new Uri(geocodeRequest.GetRequestUrl());
                GetResponse(RequestUri, x =>
                {
                    result = x.ResourceSets[0].Resources[0] as Location;
                });
                counter++;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static double RandomDouble()
        {
            Random random = new Random();
            return random.NextDouble();
        }

        /// <summary>
        ///  funkcja optymalizujaca
        /// </summary>
        /// <param name="dur">tablica odleglosci kazdy z każdym punkt</param>
        /// <param name="mode">0 - A-Z; 1 - Start = Stop</param>
        /// <param name="order">Tablica int z wynikami optymalizacji podajemy już zdeklarowany rozmiar taki jak dur</param>
        /// <returns></returns>
        public static bool AntColonyOpt(int[][] dur, int mode, ref int[] order)
        {
            if (dur.Length <= 1) return false;

            int bestTrip = 0;
            int[] bestPath;
            //int i = 0;
            int numActive = dur[1].Length;
            int[] currPath = new int[numActive];
            bool[] visited = new bool[numActive];
            const double alfa = 1.5; // The importance of the previous trails
            const double beta = 1.5; // The importance of the durations
            const double rho = 1.5;  // The decay rate of the pheromone trails
            const double asymptoteFactor = 0.997; // The sharpness of the reward as the solutions approach the best solution
            double[,] pher = new double[numActive, numActive];
            double[,] nextPher = new double[numActive, numActive];
            double[] prob = new double[numActive];
            const int numAnts = 100;
            const int numWaves = 300;

            //for (var i = 0; i < numActive; ++i) {
            //  pher[i] = new Array();
            //  nextPher[i] = new Array();
            //}

            for (var i = 0; i < numActive; ++i)
            {
                for (var j = 0; j < numActive; ++j)
                {
                    pher[i, j] = 1;
                    nextPher[i, j] = 0.0;
                }
            }

            int lastNode = 0;
            const int startNode = 0;
            int numSteps = numActive - 1;
            int numValidDests = numActive;
            if (mode == 1)
            {
                lastNode = numActive - 1;
                numSteps = numActive - 2;
                numValidDests = numActive - 1;
            }

            for (int wave = 0; wave < numWaves; ++wave)
            {
                for (int ant = 0; ant < numAnts; ++ant)
                {
                    var curr = startNode;
                    var currDist = 0;
                    for (int i = 0; i < numActive; ++i)
                    {
                        visited[i] = false;
                    }
                    currPath[0] = curr;
                    for (int step = 0; step < numSteps; ++step)
                    {
                        visited[curr] = true;
                        double cumProb = 0.0;
                        for (int next = 1; next < numValidDests; ++next)
                        {
                            if (!visited[next])
                            {
                                prob[next] = Math.Pow(pher[curr, next], alfa) *
                                  Math.Pow(dur[curr][next], 0.0 - beta);
                                cumProb += prob[next];
                            }
                        }
                        double guess = RandomDouble() * cumProb;
                        int nextI = -1;
                        for (int next = 1; next < numValidDests; ++next)
                        {
                            if (!visited[next])
                            {
                                nextI = next;
                                guess -= prob[next];
                                if (guess < 0)
                                {
                                    nextI = next;
                                    break;
                                }
                            }
                        }
                        currDist += dur[curr][nextI];
                        currPath[step + 1] = nextI;
                        curr = nextI;
                    }
                    currPath[numSteps + 1] = lastNode;
                    currDist += dur[curr][lastNode];

                    // k2-rewire:
                    int lastStep = numActive;
                    if (mode == 1)
                    {
                        lastStep = numActive - 1;
                    }
                    bool changed = true;
                    int ii = 0;
                    while (changed)
                    {
                        changed = false;
                        for (; ii < lastStep - 2 && !changed; ++ii)
                        {
                            int cost = dur[currPath[ii + 1]][currPath[ii + 2]];
                            int revCost = dur[currPath[ii + 2]][currPath[ii + 1]];
                            int iCost = dur[currPath[ii]][currPath[ii + 1]];
                            int tmp, nowCost, newCost;
                            for (int j = ii + 2; j < lastStep && !changed; ++j)
                            {
                                nowCost = cost + iCost + dur[currPath[j]][currPath[j + 1]];
                                newCost = revCost + dur[currPath[ii]][currPath[j]]
                                  + dur[currPath[ii + 1]][currPath[j + 1]];
                                if (nowCost > newCost)
                                {
                                    currDist += newCost - nowCost;
                                    // Reverse the detached road segment.
                                    for (var k = 0; k < Math.Floor((decimal)((j - ii) / 2)); ++k)
                                    {
                                        tmp = currPath[ii + 1 + k];
                                        currPath[ii + 1 + k] = currPath[j - k];
                                        currPath[j - k] = tmp;
                                    }
                                    changed = true;
                                    --ii;
                                }
                                cost += dur[currPath[j]][currPath[j + 1]];
                                revCost += dur[currPath[j + 1]][currPath[j]];
                            }
                        }
                    }

                    if (currDist < bestTrip)
                    {
                        bestPath = currPath;
                        bestTrip = currDist;
                    }

                    for (var i = 0; i <= numSteps; ++i)
                    {
                        nextPher[currPath[i], currPath[i + 1]] += (bestTrip - asymptoteFactor * bestTrip) / (numAnts * (currDist - asymptoteFactor * bestTrip));
                    }
                }
                for (var i = 0; i < numActive; ++i)
                {
                    for (var j = 0; j < numActive; ++j)
                    {
                        pher[i, j] = pher[i, j] * (1.0 - rho) + rho * nextPher[i, j];
                        nextPher[i, j] = 0.0;
                    }
                }
            }

            for (var k = 0; k < currPath.Length; ++k)
            {
                order[k] = currPath[k];
            }

            return true;
        }

        public static List<List<T>> ChunkBy<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}