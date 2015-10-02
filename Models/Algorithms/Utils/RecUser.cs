using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cykelnet.Models.Algorithms.Utils
{
    public class RecUser
    {
        private Guid userId;

        public Guid UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        private int confidence;

        public int Confidence
        {
            get { return confidence; }
            set { confidence = value; }
        }
        private List<RatingModel> ratings;

        public List<RatingModel> Ratings
        {
            get { return ratings; }
            set { ratings = value; }
        }

        private double matchingRoutes;
        private double totalRoutes;

        public RecUser(Guid id, int conf)
        {
            this.confidence = conf;
            this.userId = id;
            this.ratings = new List<RatingModel>();
        }

        public void AddRating(RatingModel rating)
        {
            this.ratings.Add(rating);
        }

        public void addConfidence(bool agree)
        {
            this.totalRoutes++;
            if (agree)
                this.matchingRoutes += 1;
            this.confidence = (int) ((matchingRoutes / totalRoutes) * 100);
        }
    }
}