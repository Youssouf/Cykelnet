using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Types;

namespace Cykelnet.Models
{
    public class RoutePointModel
    {
        public RoutePointModel()
        {
        }

        public RoutePointModel(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public RoutePointModel(SqlGeography sg)
        {
            this.latitude = sg.Lat.Value;
            this.longitude = sg.Long.Value;
        }

        public SqlGeography asSqlGeography()
        {
            SqlGeographyBuilder builder = new SqlGeographyBuilder();
            builder.SetSrid(4326);
            builder.BeginGeography(OpenGisGeographyType.Point);
            builder.BeginFigure(latitude, longitude);
            builder.EndFigure();
            builder.EndGeography();

            return builder.ConstructedGeography;
        }

        //public SqlGeography point { get; set; }

        // Only exists for the Javascript JSON Deserializer
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}