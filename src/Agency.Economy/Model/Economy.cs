using System;
using System.Collections.Generic;

namespace Agency.Economy.Model
{
    public class Economy
    {
        private readonly List<Offer> supply = new List<Offer>();
        private readonly List<Offer> demand = new List<Offer>();


        /// <summary>
        /// Use this to determine the best location for a service
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="recipe"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public INode FindBestNodeForRecipe(IEnumerable<INode> nodes, object recipe)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Use this to determine which building to grow on a lot
        /// </summary>
        /// <param name="recipes"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object FindBestRecipeForNode(IEnumerable<object> recipes, INode node)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Adds supply, which will be added to the match queue 
        /// </summary>
        public void AddSupply(Offer offer)
        {
            
        }

        /// <summary>
        /// Attempts to find demand for 
        /// </summary>
        public void AddDemand(Offer offer)
        {
            
        }

    }
}