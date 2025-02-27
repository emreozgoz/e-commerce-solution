using Basket.Application.Responses;
using Basket.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Basket.Application.Commands
{
    public class CreateShoppingCartCommand : IRequest<ShoppingCartResponse>
    {
        [JsonPropertyName("userName")]  // JSON'daki anahtar adı
        public string UserName { get; set; }

        [JsonPropertyName("items")]  // JSON'daki anahtar adı
        public List<ShoppingCartItem> Items { get; set; }

        public CreateShoppingCartCommand(string userName, List<ShoppingCartItem> items)
        {
            UserName = userName;
            Items = items;
        }
    }
}
