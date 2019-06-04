using System;
using System.Collections.Generic;
using GrainInterfaces;
using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans.Providers;

namespace Grains
{
    [StorageProvider(ProviderName="ItemStorage")]
    public class ItemGrain : Grain<ItemState>, IItemGrain
    {

        public async Task<Item> createItem() {

            await ReadStateAsync();

            State.Value = new Item
            {
                Id =  Guid.NewGuid(),
                Products = new List<Product>()
            };

            await WriteStateAsync();
            
            return State.Value;
        }
   
        public async Task<Item> GetItem()
        {
            await ReadStateAsync();
            
            if (State.Value != null) return State.Value;
            
            //Todo: What should be returned when not an item?
            return null;
        }

        public async Task AddProduct(Product product)
        {
            await ReadStateAsync();

            if (State.Value == null)
            {
                State = new CartState();
            }
            
            State.Value.Products.Add(product);
            await WriteStateAsync();
        }
        
        
    }

}
