﻿using Dannnno.StardewMods.Abstraction;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Dannnno.StardewMods.Predictor.Geodes
{
    /// <summary>
    /// The set of directions we can predict in
    /// </summary>
    public enum PredictionDirectionEnum
    {
        Forwards,
        Backwards
    }

    /// <summary>
    /// Predicts what new geode objects will be found
    /// </summary>
    public class GeodePredictor
    {
        #region fields
        private IGeodeService<IStardewObjectProvider> geodeService;
        private IStardewObjectProvider objectProvider;
        private Lazy<IList<StardewValley.Object>> geodeList;
        #endregion

        #region properties
        /// <summary>
        /// Get or set the service used to retrieve the geodes
        /// </summary>
        public IGeodeService<IStardewObjectProvider> GeodeService
        {
            get => geodeService;
            set
            {
                geodeService = value;
                InitializeCache();
            }
        }

        /// <summary>
        /// Get the list of geodes that this predictor can work on
        /// </summary>
        public IList<StardewValley.Object> GeodeList { get => geodeList.Value; }

        /// <summary>
        /// Get or set the provider of geode objects
        /// </summary>
        public IStardewObjectProvider ObjectProvider
        {
            get => objectProvider;
            set
            {
                objectProvider = value;
                InitializeCache();
            }
        }

        /// <summary>
        /// Get or set how to calculate geode contents
        /// </summary>
        public IGeodeTreasureCalculator GeodeCalculator { get; set; }

        /// <summary>
        /// Get or set the associated game
        /// </summary>
        public IStardewGame Game { get; set; }

        /// <summary>
        /// Get or set the associated monitor
        /// </summary>
        public IMonitor Monitor { get; set; }

        /// <summary>
        /// Get or set the predictions we've made
        /// </summary>
        private IDictionary<uint, IDictionary<StardewValley.Object, StardewValley.Object>> CachedPredictions { get; set; }
        #endregion

        /// <summary>
        /// Create a new predictor
        /// </summary>
        /// <param name="service">The service to use to predict geodes</param>
        /// <param name="provider">The provider to retrieve objects from</param>
        /// <param name="game">The game this predictor is associated with</param>
        /// <param name="calculator">The calculator that will return the geode's treasure</param>
        /// <param name="monitor">The monitor to log to</param>
        public GeodePredictor(IGeodeService<IStardewObjectProvider> service, IStardewObjectProvider provider, IStardewGame game, IGeodeTreasureCalculator calculator, IMonitor monitor = null)
        {
            GeodeService = service;
            ObjectProvider = provider;
            Game = game;
            Monitor = monitor;
            GeodeCalculator = calculator;
            InitializeCache();
        }


        /// <summary>
        /// Empty the cache and start over
        /// </summary>
        private void InitializeCache()
        {
            CachedPredictions = new Dictionary<uint, IDictionary<StardewValley.Object, StardewValley.Object>>();
            geodeList = new Lazy<IList<StardewValley.Object>>(() => GeodeService.RetrieveGeodes(ObjectProvider).ToList());
        }

        /// <summary>
        /// Predict the treasures that a geode `distance` items away will return
        /// </summary>
        /// <param name="distance">How far ahead to look</param>
        /// <param name="direction">The direction to look</param>
        /// <returns>For each kind of geode we can predict, the associated result</returns>
        public IDictionary<StardewValley.Object, StardewValley.Object> PredictTreasureFromGeodeAtDistance(uint distance = 1, PredictionDirectionEnum direction = PredictionDirectionEnum.Forwards)
        {
            uint actualSearchIndex = direction switch
            {
                PredictionDirectionEnum.Backwards => distance > Game.GeodeCount ? Game.GeodeCount : Game.GeodeCount - distance,
                PredictionDirectionEnum.Forwards => Game.GeodeCount + distance,
                _ => Game.GeodeCount
            };

            return PredictTreasureFromGeodeAtIndex(actualSearchIndex);
        }

        /// <summary>
        /// Predict the treasures that will come from geodes in a span ahead and behind of our current count
        /// </summary>
        /// <param name="distanceAhead">How far ahead to peek</param>
        /// <param name="distanceBehind">How far behind to look</param>
        /// <returns>The treasures found</returns>
        public IEnumerable<IDictionary<StardewValley.Object, StardewValley.Object>> PredictTreasureFromGeodeByRangeDistance(uint distanceAhead, uint distanceBehind)
        {
            uint startIndex = Game.GeodeCount < distanceBehind ? Game.GeodeCount : Game.GeodeCount - distanceBehind;
            uint endIndex = Game.GeodeCount + distanceAhead;

            return PredictTreasureFromGeodesInRange(startIndex, endIndex);
        }

        /// <summary>
        /// Predict the treasures that a geode at a given count will return
        /// </summary>
        /// <param name="actualGeodeCount">The geode count to check</param>
        /// <returns>The treasures found</returns>
        private IDictionary<StardewValley.Object, StardewValley.Object> PredictTreasureFromGeodeAtIndex(uint actualGeodeCount)
        {
            return PredictTreasureFromGeodesInRange(actualGeodeCount, actualGeodeCount).First();
        }

        /// <summary>
        /// Predict the treasures that geodes within a range will return
        /// </summary>
        /// <param name="firstGeodeCount">The first to check</param>
        /// <param name="lastGeodeCount">The last to check</param>
        /// <returns>The treasures found</returns>
        private IEnumerable<IDictionary<StardewValley.Object, StardewValley.Object>> PredictTreasureFromGeodesInRange(uint firstGeodeCount, uint lastGeodeCount)
        {
            Contract.Requires(firstGeodeCount <= lastGeodeCount, "The first count must not be greater than the last count");

            var results = new List<IDictionary<StardewValley.Object, StardewValley.Object>>();

            using (Game.WithTemporaryChanges(Monitor))
            {
                for (; firstGeodeCount < lastGeodeCount; ++firstGeodeCount)
                {
                    if (!CachedPredictions.ContainsKey(firstGeodeCount))
                    {
                        // Temporarily modify the current geode count
                        Game.GeodeCount = firstGeodeCount;
                        CachedPredictions[firstGeodeCount] = GeodeList.ToDictionary(geodeKind => geodeKind,
                                                                                    geodeKind => GeodeCalculator.GetTreasureFromGeode(geodeKind));
                    }

                    results.Add(CachedPredictions[firstGeodeCount]);
                }
            }

            // Don't use yield return because we don't want to hold the context manager for too long
            return results;
        }
    }
}
