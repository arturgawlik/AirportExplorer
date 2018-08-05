using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace AirportExplorer.Pages
{
    public class IndexModel : PageModel
    {
        public string MapboxAccessToken { get; set; }
        public IHostingEnvironment _hostingEnvironment { get; set; }
        
        public IndexModel(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            MapboxAccessToken = configuration["Mapbox:AccessToken"];
            _hostingEnvironment = hostingEnvironment;
        }
        
        public void OnGet()
        {

        }

        public IActionResult OnGetAirports()
        {
            var configuration = new Configuration
            {
                BadDataFound = context => { }
            };
            
            using(var sr = new StreamReader(Path.Combine(_hostingEnvironment.WebRootPath, "airports.dat")))
            using (var reader = new CsvReader(sr, configuration))
            {
                var featureCollection = new FeatureCollection();
                while (reader.Read())
                {
                    var name = reader.GetField<string>(1);
                    var iataCode = reader.GetField<string>(4);
                    var latitude = reader.GetField<double>(6);
                    var longitude = reader.GetField<double>(7);
                    
                    featureCollection.Features.Add(new Feature(new Point(new Position(latitude, longitude)),
                        new Dictionary<string, object>
                        {
                            {"name", name},
                            {"iataCode", iataCode}
                        }));
                }
                
                return new JsonResult(featureCollection);
            }
        }
    }
}
