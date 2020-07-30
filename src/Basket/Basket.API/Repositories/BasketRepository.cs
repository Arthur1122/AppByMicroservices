using Basket.API.Data.Interfaces;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IBasketContext _basketContext;

        public BasketRepository(IBasketContext basketContext)
        {
            this._basketContext = basketContext;
        }

        public async Task<BasketCart> GetBasket(string userName)
        {
            var basket = await _basketContext.Redis
                                              .StringGetAsync(userName);
            if(basket.IsNullOrEmpty)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<BasketCart>(basket);
        }

        public async Task<BasketCart> UpdateBasket(BasketCart basketCart)
        {
            var updateBasket = await _basketContext.Redis
                                             .StringSetAsync(basketCart.UserName, JsonConvert.SerializeObject(basketCart));
            if(!updateBasket)
            {
                return null;
            }
            return await GetBasket(basketCart.UserName);
        }

        public async Task<bool> DeleteBasket(string userName)
        {
            return await _basketContext.Redis
                                       .KeyDeleteAsync(userName);
        }

        
    }
}
